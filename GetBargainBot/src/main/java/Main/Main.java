package Main;

import Bot.GBbot;
import DB.ProductDao;
import DB.UserDAO;
import org.quartz.SchedulerException;
import org.telegram.telegrambots.ApiContextInitializer;
import org.telegram.telegrambots.TelegramBotsApi;
import org.telegram.telegrambots.exceptions.TelegramApiException;
import org.telegram.telegrambots.exceptions.TelegramApiRequestException;

import java.sql.SQLException;

public class Main {
    public static GBbot b;
    public static void main(String[] args) throws SQLException, TelegramApiException {
        ApiContextInitializer.init();
        TelegramBotsApi telegramBotsApi = new TelegramBotsApi();
        try {
            b = new GBbot();
            telegramBotsApi.registerBot(b);
            ProductDao dao = new ProductDao();
            b.sendMessage(String.join("\n~~~~~~~~~~~~~~~~~~~~\n",dao.getFavCategoriesProducts(544770546)),544770546);

        } catch (TelegramApiRequestException | SchedulerException e) {

        }
    }
}
