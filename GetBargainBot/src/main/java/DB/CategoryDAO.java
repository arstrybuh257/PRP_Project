package DB;

import KeyBoard.InlineKeyboardBuilder;
import org.telegram.telegrambots.api.methods.send.SendMessage;
import org.telegram.telegrambots.api.objects.replykeyboard.InlineKeyboardMarkup;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class CategoryDAO {
    Connection con;

    public CategoryDAO() {
        try {
            con = DBConnection.getConnection();
        } catch (SQLException e) {
            System.out.println("Error getting connection!");
        }
    }

    public HashMap<String,Integer> getSuperCategoriesAsMap() throws SQLException {

        String query = "SELECT id,name from SuperCategories\n";
        PreparedStatement statement = con.prepareStatement(query);
        ResultSet resultSet = statement.executeQuery();
        HashMap<String,Integer> map = new HashMap<>();
        while (resultSet.next()) {
            map.put(resultSet.getString("name"),resultSet.getInt("id"));
        }
        return map;
    }

    public InlineKeyboardMarkup getSuperCategories() throws SQLException {
        InlineKeyboardMarkup markup;
        InlineKeyboardBuilder builder = InlineKeyboardBuilder.create();
        int i = 0;
            for (HashMap.Entry<String,Integer> e : getSuperCategoriesAsMap().entrySet()) {
                if(i==0){
                    builder =  builder.row();
                }
                i++;
                builder.button(e.getKey(),"s"+e.getValue());
                if(i==2){
                    builder =  builder.endRow();
                    i=0;
                }
            }
            builder.row().button("Назад","sBack").endRow();
            markup = builder.getKeyBoard();
            return markup;
    }

    public SendMessage displaySuperCategories(long tel_id, String message) throws SQLException {
        return new SendMessage().setReplyMarkup(getSuperCategories()).setText(message).setChatId(tel_id);
    }

    public InlineKeyboardMarkup getCategories(int id) throws SQLException {
        InlineKeyboardMarkup markup;
        String query = "SELECT Categories.id,Categories.Name from Categories\n"+
                "join SuperCategories on SuperCategories.Id = Categories.SuperCategoryId "+
                "where Categories.SuperCategoryId=?";
        PreparedStatement statement = con.prepareStatement(query);
        statement.setInt(1,id);
        ResultSet resultSet = statement.executeQuery();
        InlineKeyboardBuilder builder = InlineKeyboardBuilder.create();
        int i = 0;
        while (resultSet.next()) {
            if(i==0){
                builder =  builder.row();
            }
            i++;
            builder.button(resultSet.getString("name"),"c"+resultSet.getInt("id"));
            if(i==2){
                builder =  builder.endRow();
                i=0;
            }
        }
        builder.row().button("Назад","cBack").endRow();
        markup = builder.getKeyBoard();
        return markup;
    }
}
