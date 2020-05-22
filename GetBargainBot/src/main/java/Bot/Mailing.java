package Bot;

import DB.UserDAO;
import KeyBoard.InlineKeyboardBuilder;
import Main.Main;
import org.quartz.Job;
import org.quartz.JobExecutionContext;
import org.quartz.JobExecutionException;
import org.telegram.telegrambots.exceptions.TelegramApiException;

import java.sql.SQLException;

public class Mailing implements Job {
    UserDAO dao = new UserDAO();
    @Override
    public void execute(JobExecutionContext jobExecutionContext) throws JobExecutionException {
        try {
            for(long l : dao.getAllIds()){
                Main.b.sendMessage("ище адна пасылачка. вот распишитесь пажулсто. бонжур мерси",l);
            }
        } catch (TelegramApiException | SQLException e) {
            e.printStackTrace();
        }
    }
}
