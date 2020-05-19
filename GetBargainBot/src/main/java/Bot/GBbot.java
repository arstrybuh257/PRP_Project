package Bot;

import DB.ProductDao;
import Enums.Status;
import DB.UserDAO;
import Enums.Step;
import KeyBoard.InlineKeyboardBuilder;
import KeyBoard.PlainKeyboardBuilder;
import org.telegram.telegrambots.api.methods.send.SendMessage;
import org.telegram.telegrambots.api.objects.Message;
import org.telegram.telegrambots.api.objects.Update;
import org.telegram.telegrambots.bots.TelegramLongPollingBot;
import org.telegram.telegrambots.exceptions.TelegramApiException;

import java.sql.SQLException;

public class GBbot extends TelegramLongPollingBot {
    UserDAO dao = new UserDAO();
    ProductDao dao2 = new ProductDao();

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
                    if (messageText.equals("Назад")) {
                        dao.changeStep(tel_id, null);
                    }
                    if (dao.getCurrentStep(tel_id) == null) {
                        execute(PlainKeyboardBuilder.create().row().button("Найти").button("Каталог").endRow().row().button("Сменить режим рассылки").endRow().setChatId(tel_id).setText("Меню:").build());
                        dao.changeStep(tel_id, Step.MENU);
                    } else if (dao.getCurrentStep(tel_id) == Step.SEARCH) {
                        sendMessage("Найдено товаров: " + dao2.searchProducts(messageText).size() + "\n" + String.join("\n~~~~~~~~~~~~~\n", dao2.searchProducts(messageText)), tel_id);
                    } else if (messageText.equals("Найти")) {
                        execute(PlainKeyboardBuilder.create().row().button("Назад").endRow().setChatId(tel_id).setText("Введите товар для поиска:").build());
                        dao.changeStep(tel_id, Step.SEARCH);
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
            }
        }
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

    void allOperations() {

    }

    public String getBotUsername() {
        return "GetDiscountsBot";
    }

    public String getBotToken() {
        return "1132646013:AAHgskgu7ZlM-iGKCPJueonEzQLF8Cdf4Iw";
    }
}
