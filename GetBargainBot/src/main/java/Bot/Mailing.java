package Bot;

import DB.ProductDao;
import DB.UserDAO;
import KeyBoard.InlineKeyboardBuilder;
import Main.Main;
import org.quartz.Job;
import org.quartz.JobExecutionContext;
import org.telegram.telegrambots.exceptions.TelegramApiException;

import java.sql.SQLException;

public class Mailing implements Job {
    UserDAO dao = new UserDAO();
    ProductDao dao2 = new ProductDao();
    @Override
    public void execute(JobExecutionContext jobExecutionContext) {
        try {
            for(long l : dao.getAllIds()){
                Main.b.sendMessage("<strong>ЕЖЕНЕДЕЛЬНАЯ РАССЫЛКА:</strong>\n"+String.join("\n~~~~~~~~~~~~~~~~~~~~\n",dao2.getFavCategoriesProducts(l)),l);
            }
        } catch (TelegramApiException | SQLException e) {
            e.printStackTrace();
        }
    }
}
