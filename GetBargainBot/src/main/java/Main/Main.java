package Main;

import Bot.GBbot;
import DB.UserDAO;
import org.quartz.SchedulerException;
import org.telegram.telegrambots.ApiContextInitializer;
import org.telegram.telegrambots.TelegramBotsApi;
import org.telegram.telegrambots.exceptions.TelegramApiRequestException;

import java.sql.SQLException;

public class Main {
    public static GBbot b;
    public static void main(String[] args) throws SQLException {
        ApiContextInitializer.init();
        TelegramBotsApi telegramBotsApi = new TelegramBotsApi();
        try {
            b = new GBbot();
            telegramBotsApi.registerBot(b);
        } catch (TelegramApiRequestException | SchedulerException e) {

        }
    }
}
