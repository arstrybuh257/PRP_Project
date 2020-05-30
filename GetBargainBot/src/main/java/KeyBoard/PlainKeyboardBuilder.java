package KeyBoard;

import org.telegram.telegrambots.api.methods.send.SendMessage;
import org.telegram.telegrambots.api.objects.replykeyboard.InlineKeyboardMarkup;
import org.telegram.telegrambots.api.objects.replykeyboard.ReplyKeyboardMarkup;
import org.telegram.telegrambots.api.objects.replykeyboard.buttons.InlineKeyboardButton;
import org.telegram.telegrambots.api.objects.replykeyboard.buttons.KeyboardButton;
import org.telegram.telegrambots.api.objects.replykeyboard.buttons.KeyboardRow;

import java.util.ArrayList;
import java.util.List;

public class PlainKeyboardBuilder {
    private Long chatId;
    private String text;

    ReplyKeyboardMarkup replyKeyboardMarkup = new ReplyKeyboardMarkup();
    private List<KeyboardRow> keyboard = new ArrayList<>();
    private KeyboardRow row = null;

    private PlainKeyboardBuilder() {}

    public static PlainKeyboardBuilder create() {
        PlainKeyboardBuilder builder = new PlainKeyboardBuilder();
        return builder;
    }

    public static PlainKeyboardBuilder create(Long chatId) {
        PlainKeyboardBuilder builder = new PlainKeyboardBuilder();
        builder.setChatId(chatId);
        return builder;
    }

    public PlainKeyboardBuilder setText(String text) {
        this.text = text;
        return this;
    }

    public PlainKeyboardBuilder setChatId(Long chatId) {
        this.chatId = chatId;
        return this;
    }

    public PlainKeyboardBuilder row() {
        this.row = new KeyboardRow();
        return this;
    }

    public PlainKeyboardBuilder button(String text) {
        row.add(new KeyboardButton(text));
        return this;
    }

    public PlainKeyboardBuilder endRow() {
        this.keyboard.add(this.row);
        this.row = null;
        return this;
    }


    public SendMessage build() {
        SendMessage message = new SendMessage();
        message.setChatId(chatId);
        message.setText(text);
        replyKeyboardMarkup.setResizeKeyboard(true).setKeyboard(keyboard);
        message.setReplyMarkup(replyKeyboardMarkup);
        return message;
    }
}
