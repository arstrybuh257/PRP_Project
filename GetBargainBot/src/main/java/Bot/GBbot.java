package Bot;

import DB.ProductDao;
import DB.CategoryDAO;
import Enums.Paging;
import Enums.Status;
import DB.UserDAO;
import Enums.Step;
import KeyBoard.InlineKeyboardBuilder;
import KeyBoard.PlainKeyboardBuilder;
import org.quartz.*;
import org.quartz.impl.StdSchedulerFactory;
import org.telegram.telegrambots.api.methods.AnswerCallbackQuery;
import org.telegram.telegrambots.api.methods.send.SendMessage;
import org.telegram.telegrambots.api.methods.send.SendPhoto;
import org.telegram.telegrambots.api.methods.updatingmessages.DeleteMessage;
import org.telegram.telegrambots.api.methods.updatingmessages.EditMessageReplyMarkup;
import org.telegram.telegrambots.api.methods.updatingmessages.EditMessageText;
import org.telegram.telegrambots.api.objects.Message;
import org.telegram.telegrambots.api.objects.Update;
import org.telegram.telegrambots.api.objects.replykeyboard.ReplyKeyboardRemove;
import org.telegram.telegrambots.bots.TelegramLongPollingBot;
import org.telegram.telegrambots.exceptions.TelegramApiException;

import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;
import java.util.stream.Collectors;

import static org.quartz.CronScheduleBuilder.dailyAtHourAndMinute;
import static org.quartz.SimpleScheduleBuilder.simpleSchedule;
import static org.quartz.TriggerBuilder.newTrigger;

public class GBbot extends TelegramLongPollingBot {
    UserDAO dao = new UserDAO();
    ProductDao dao2 = new ProductDao();
    CategoryDAO dao3 = new CategoryDAO();
    Scheduler scheduler = new StdSchedulerFactory().getScheduler();


    public GBbot() throws SchedulerException {
         new SendMessage().setParseMode("Markdown");
        JobDetail job = JobBuilder.newJob(Mailing.class)
                .withIdentity("GBbot").build();
        Trigger trigger = newTrigger()
                .withIdentity("trigger")
                .withSchedule(dailyAtHourAndMinute(23, 56))
                .build();
        scheduler.start();
        scheduler.scheduleJob(job, trigger);
    }

    public void sendMessage(String text, long id) throws TelegramApiException {
        execute(new SendMessage().setChatId(id).setText(text));
    }

    public void onUpdateReceived(Update update) {
        if (update.hasMessage()) {
            Message updateMessage = update.getMessage();
            String messageText = updateMessage.getText();
            long tel_id = updateMessage.getChatId();
            if (messageText.equals("/start")) {
                try {
                    sendMessage("Приветствую", tel_id);
                } catch (TelegramApiException e) {
                    e.printStackTrace();
                }
            } else if (messageText.equals("/exit")) {
                try {
                    dao.Exit(tel_id);
                } catch (SQLException e) {
                    e.printStackTrace();
                }
            }
            try {
                if (Status.valueOf(dao.getStatus(dao.getChatIdByEmail(messageText)).toUpperCase()) == Status.SUCCESS && dao.userLoggedIn(messageText)) {
                    sendMessage("Пользователь уже вошел", tel_id);
                    return;
                } else {
                    Login(tel_id, messageText);
                }
                if (Status.valueOf(dao.getStatus(tel_id).toUpperCase()) == Status.SUCCESS) {
                    if(tryParse(messageText.substring(1))){
                        String[] arr = dao2.getProductById(messageText.substring(1));
                        String caption = String.format("%s, ціна - %s грн., стара ціна - <s>%s</s> грн. ",arr[0],arr[1],arr[2]);
                        SendPhoto mes = new SendPhoto().setParseMode("HTML").setCaption(caption).setPhoto(arr[3]).setChatId(tel_id);
                        sendPhoto(mes);
                        //execute(new SendMessage().setChatId(tel_id).enableMarkdown(true).enableHtml(true).setText(caption));
                        System.out.println(arr[3]);
                        return;
                    }
                    if (messageText.equals("Назад")) {
                        dao.changeStep(tel_id, null);
                    }
                    if (dao.getCurrentStep(tel_id) == null) {
                        execute(PlainKeyboardBuilder.create().row().button("\uD83D\uDD0DНайти\uD83D\uDD0D").button("\uD83D\uDCDAКаталог\uD83D\uDCDA").endRow().setChatId(tel_id).setText("Меню:").build());
                        dao.changeStep(tel_id, Step.MENU);
                    } else if (dao.getCurrentStep(tel_id) == Step.SEARCH) {
                        System.out.println(dao2.searchProducts(messageText).size() + "\n" + String.join("\n~~~~~~~~~~~~~\n", dao2.searchProducts(messageText)).length());
                        sendMessage("Найдено товаров: " + dao2.searchProducts(messageText).size() + "\n" + String.join("\n~~~~~~~~~~~~~\n", dao2.searchProducts(messageText)), tel_id);
                    } else if (messageText.equals("\uD83D\uDD0DНайти\uD83D\uDD0D")) {
                        execute(PlainKeyboardBuilder.create().row().button("Назад").endRow().setChatId(tel_id).setText("Введите товар для поиска:").build());
                        dao.changeStep(tel_id, Step.SEARCH);
                    } else if (messageText.equals("\uD83D\uDCDAКаталог\uD83D\uDCDA")) {
                        execute(new SendMessage(tel_id, "Клавиатура скрыта!").setReplyMarkup(new ReplyKeyboardRemove()));
                        execute(dao3.displaySuperCategories(tel_id, messageText));
                    }
                }

            } catch (SQLException | TelegramApiException e) {
                e.printStackTrace();
            }
        }
        if (update.hasCallbackQuery()) {
            if (update.getCallbackQuery().getData().equals("chMail")) {
                try {
                    dao.changeStatus(update.getCallbackQuery().getMessage().getChatId(), Status.NONEXISTANCE);
                    Auth(update.getCallbackQuery().getMessage().getChatId());
                } catch (SQLException | TelegramApiException e) {
                    e.printStackTrace();
                }
            } else if (update.getCallbackQuery().getData().startsWith("s")) {
                try {
                    if (update.getCallbackQuery().getData().endsWith("Back")) {
                        execute(PlainKeyboardBuilder.create().row().button("\uD83D\uDD0DНайти\uD83D\uDD0D").button("\uD83D\uDCDAКаталог\uD83D\uDCDA").endRow().setChatId(update.getCallbackQuery().getMessage().getChatId()).setText("Меню:").build());
                        deleteMessage(new DeleteMessage().setMessageId(update.getCallbackQuery().getMessage().getMessageId()).setChatId(String.valueOf(update.getCallbackQuery().getMessage().getChatId())));
                    } else
                        editMessageReplyMarkup(new EditMessageReplyMarkup().setMessageId(update.getCallbackQuery().getMessage().getMessageId()).setChatId(update.getCallbackQuery().getMessage().getChatId()).setReplyMarkup(dao3.getCategories(Integer.parseInt(update.getCallbackQuery().getData().substring(1)))));
                } catch (TelegramApiException | SQLException e) {
                    e.printStackTrace();
                }
            } else if (update.getCallbackQuery().getData().startsWith("c")) {
                try {
                    if (update.getCallbackQuery().getData().endsWith("Back")) {
                        editMessageReplyMarkup(new EditMessageReplyMarkup().setMessageId(update.getCallbackQuery().getMessage().getMessageId()).setChatId(update.getCallbackQuery().getMessage().getChatId()).setReplyMarkup(dao3.getSuperCategories()));
                    } else {
                        dao.setPageStart(update.getCallbackQuery().getMessage().getChatId());
                        System.out.println(update.getCallbackQuery().getData());
                        List<String> list = dao2.searchProducts(Integer.parseInt(update.getCallbackQuery().getData().substring(1)));
                        list = list.stream().skip(25 * dao.getPage(update.getCallbackQuery().getMessage().getChatId())).limit(25).collect(Collectors.toList());
                        editMessageText(new EditMessageText().enableHtml(true).setMessageId(update.getCallbackQuery().getMessage().getMessageId()).setChatId(update.getCallbackQuery().getMessage().getChatId()).setText(String.join("\n~~~~~~~~~~~~~\n", list)));
                        editMessageReplyMarkup(new EditMessageReplyMarkup().setMessageId(update.getCallbackQuery().getMessage().getMessageId()).setChatId(update.getCallbackQuery().getMessage().getChatId()).setReplyMarkup(InlineKeyboardBuilder.create().row().button("<", "<" + update.getCallbackQuery().getData().substring(1)).button(">", ">" + update.getCallbackQuery().getData().substring(1)).endRow().row().button("Назад", "pBack" + dao3.getSuperBySub(update.getCallbackQuery().getData().substring(1))).endRow().getKeyBoard()));
                    }

                } catch (org.telegram.telegrambots.exceptions.TelegramApiValidationException ex){
                    try {
                        AnswerCallbackQuery answ = new AnswerCallbackQuery().setCallbackQueryId(update.getCallbackQuery().getId()).setText("Немає товарів у категорії!");
                        answ.setCacheTime(2000);
                        answerCallbackQuery(answ);
                    } catch (TelegramApiException e) {
                        e.printStackTrace();
                    }
                }catch (TelegramApiException | SQLException e) {
                    e.printStackTrace();
                }
            } else if (update.getCallbackQuery().getData().contains(">")) {
                try {
                    List<String> list = dao2.searchProducts(Integer.parseInt(update.getCallbackQuery().getData().substring(1)));
                    dao.setPage(Paging.NEXT, update.getCallbackQuery().getMessage().getChatId());
                    list = list.stream().skip(20 * dao.getPage(update.getCallbackQuery().getMessage().getChatId())).limit(25).collect(Collectors.toList());
                    editMessageText(new EditMessageText().setMessageId(update.getCallbackQuery().getMessage().getMessageId()).setChatId(update.getCallbackQuery().getMessage().getChatId()).setText(String.join("\n~~~~~~~~~~~~~\n", list)));
                    editMessageReplyMarkup(new EditMessageReplyMarkup().setMessageId(update.getCallbackQuery().getMessage().getMessageId()).setChatId(update.getCallbackQuery().getMessage().getChatId()).setReplyMarkup(InlineKeyboardBuilder.create().row().button("<", "<" + update.getCallbackQuery().getData().substring(1)).button(">", ">" + update.getCallbackQuery().getData().substring(1)).endRow().row().button("Назад", "pBack" + dao3.getSuperBySub(update.getCallbackQuery().getData().substring(1))).endRow().getKeyBoard()));
                } catch (SQLException | TelegramApiException e) {
                    e.printStackTrace();
                }
            } else if (update.getCallbackQuery().getData().contains("<")) {
                try {
                    dao.setPage(Paging.PREV, update.getCallbackQuery().getMessage().getChatId());
                    List<String> list = dao2.searchProducts(Integer.parseInt(update.getCallbackQuery().getData().substring(1)));
                    list = list.stream().skip(20 * dao.getPage(update.getCallbackQuery().getMessage().getChatId())).limit(25).collect(Collectors.toList());
                    editMessageText(new EditMessageText().setMessageId(update.getCallbackQuery().getMessage().getMessageId()).setChatId(update.getCallbackQuery().getMessage().getChatId()).setText(String.join("\n~~~~~~~~~~~~~\n", list)));
                    editMessageReplyMarkup(new EditMessageReplyMarkup().setMessageId(update.getCallbackQuery().getMessage().getMessageId()).setChatId(update.getCallbackQuery().getMessage().getChatId()).setReplyMarkup(InlineKeyboardBuilder.create().row().button("<", "<" + update.getCallbackQuery().getData().substring(1)).button(">", ">" + update.getCallbackQuery().getData().substring(1)).endRow().row().button("Назад", "pBack" + dao3.getSuperBySub(update.getCallbackQuery().getData().substring(1))).endRow().getKeyBoard()));
                    System.out.println(update.getCallbackQuery().getData().substring(1));
                } catch (SQLException | TelegramApiException e) {
                    e.printStackTrace();
                }
            } else if (update.getCallbackQuery().getData().startsWith("p") && update.getCallbackQuery().getData().contains("Back")) {
                try {
                    dao.setPage(Paging.PREV, update.getCallbackQuery().getMessage().getChatId());
                    int categoryId = Integer.parseInt(update.getCallbackQuery().getData().substring(5));
                    System.out.println(categoryId);
                    editMessageReplyMarkup(new EditMessageReplyMarkup().setMessageId(update.getCallbackQuery().getMessage().getMessageId()).setChatId(update.getCallbackQuery().getMessage().getChatId()).setReplyMarkup(dao3.getCategories(Integer.parseInt(update.getCallbackQuery().getData().substring(5)))));
                } catch (TelegramApiException e) {
                    e.printStackTrace();
                } catch (SQLException e) {
                    e.printStackTrace();
                }
            }
        }
        System.out.println("end");
    }

    private void Auth(long tel_id) throws SQLException, TelegramApiException {
        Status status = Status.valueOf(dao.getStatus(tel_id).toUpperCase());
        if (!dao.userAuthenificated(tel_id) || status == Status.NONEXISTANCE) {
            sendMessage("Введите почту!", tel_id);
            dao.changeStatus(tel_id, Status.EMAIL);
        } else if (status == Status.EMAIL) {
            sendMessage("Введите корректную почту!", tel_id);
        } else if (status == Status.PASSWORD) {
            SendMessage message = InlineKeyboardBuilder.create(tel_id)
                    .setText("Введите пароль!")
                    .row().button("Сменить почту!", "chMail").endRow().build();
            execute(message);
        }
    }

    private void Login(long tel_id, String str) throws SQLException, TelegramApiException {
        Status status = Status.valueOf(dao.getStatus(tel_id).toUpperCase());
        if (status == Status.EMAIL) {
            if (dao.userExists(str)) {
                dao.changeStatus(tel_id, Status.PASSWORD).setEmail(tel_id, str);
            }
        } else if (status == Status.PASSWORD) {
            if (dao.Verify(dao.getEmail(tel_id), str)) {
                dao.changeStatus(tel_id, Status.SUCCESS);
                sendMessage("Вы успешно вошли!", tel_id);
                dao.SignUp(dao.getEmail(tel_id), tel_id);
            }
        }
        Auth(tel_id);
    }

    public String getBotUsername() {
        return "GetDiscountsBot";
    }

    public String getBotToken() {
        return "1132646013:AAHgskgu7ZlM-iGKCPJueonEzQLF8Cdf4Iw";
    }

    boolean tryParse(String str) {
        try  {
            Integer.parseInt(str);
            return true;
        } catch (NumberFormatException e) {
            return false;
        }
    }

}
