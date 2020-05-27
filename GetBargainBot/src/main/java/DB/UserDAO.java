package DB;

import Enums.Paging;
import Enums.Status;
import Enums.Step;
import com.microsoft.sqlserver.jdbc.SQLServerException;

import java.sql.*;
import java.util.ArrayList;
import java.util.List;

@SuppressWarnings("SqlDialectInspection")
public class UserDAO {
    Connection con;

    public UserDAO() {
        try {
            con = DBConnection.getConnection();
        } catch (SQLException e) {
            System.out.println(e.toString());
        }
    }

    public List<Long> getAllIds() throws SQLException {
        ArrayList<Long> list = new ArrayList<>();
        ResultSet resultSet;
        String query = "select chat_id from users";
        PreparedStatement statement = con.prepareStatement(query);
        resultSet = statement.executeQuery();
        while(resultSet.next()){
            list.add(Long.parseLong(resultSet.getString("chat_id")));
        }
            return list;
    }

    public String getStatus(long id) throws SQLException {
        ResultSet resultSet;
        String query = "select status from auth where chat_id = ?";
        PreparedStatement statement = con.prepareStatement(query);
        statement.setString(1, String.valueOf(id));
        resultSet = statement.executeQuery();
        if (resultSet.next()) {
            return resultSet.getString("status");
        } else {
            return "NONEXISTANCE";
        }
    }

    public UserDAO changeStatus(long id, Status status) throws SQLException {
        if (userAuthenificated(id)) {
            String query = " UPDATE Auth\n" +
                    "    SET status = ? " +
                    "    WHERE chat_id = ?";
            PreparedStatement statement = con.prepareStatement(query);
            statement.setString(1, String.valueOf(status));
            statement.setString(2, String.valueOf(id));
            statement.execute();
        } else {
            String query = " INSERT  into Auth(chat_id,status) values(?,?) ";
            PreparedStatement statement = con.prepareStatement(query);
            statement.setString(1, String.valueOf(id));
            statement.setString(2, String.valueOf(status));
            statement.execute();
        }
        return this;
    }

    public void changeStep(long id, Step step) throws SQLException {
        String query = " UPDATE Auth\n" +
                "    SET step = ? " +
                "    WHERE chat_id = ?";
        PreparedStatement statement = con.prepareStatement(query);
        statement.setString(2, String.valueOf(id));
        if (step == null)
            statement.setString(1, null);
        else
            statement.setString(1, String.valueOf(step));
        statement.execute();
    }

    public String getEmail(long chat_id) throws SQLException {
        String query = "select email from auth where chat_id = ?";
        PreparedStatement statement = con.prepareStatement(query);
        statement.setString(1, String.valueOf(chat_id));
        ResultSet resultSet = statement.executeQuery();
        if (resultSet.next()) {
            return resultSet.getString("email");
        } else {
            return "null";
        }
    }

    public long getChatIdByEmail(String email) throws SQLException {
        String query = "select chat_id from auth where email = ?";
        PreparedStatement statement = con.prepareStatement(query);
        statement.setString(1, email);
        ResultSet resultSet = statement.executeQuery();
        if (resultSet.next()) {
            return Long.parseLong(resultSet.getString("chat_id"));
        } else {
            return -1;
        }
    }

    public void setEmail(long chat_id, String email) throws SQLException {
        String query = " UPDATE auth\n" +
                "    SET email = ? " +
                "    WHERE chat_id = ?";
        PreparedStatement statement = con.prepareStatement(query);
        statement.setString(1, String.valueOf(email));
        statement.setString(2, String.valueOf(chat_id));
        statement.execute();
    }

    public int getPage(long chat_id) throws SQLException {
        String query = "select page from auth where chat_id = ?";
        PreparedStatement statement = con.prepareStatement(query);
        statement.setString(1, String.valueOf(chat_id));
        ResultSet resultSet = statement.executeQuery();
        resultSet.next();
        if (resultSet.getInt("page")==-1)
            return -1;
        else
            return resultSet.getInt("page");
    }

    public Step getCurrentStep(long chat_id) throws SQLException {
        String query = "select step from auth where chat_id = ?";
        PreparedStatement statement = con.prepareStatement(query);
        statement.setString(1, String.valueOf(chat_id));
        ResultSet resultSet = statement.executeQuery();
        resultSet.next();
        if (resultSet.getString("step")==null)
            return null;
        else
            return Step.valueOf(resultSet.getString("step").toUpperCase());
    }

    public void setPage(Paging direction, long chat_id) throws SQLException {
        if(direction==Paging.PREV && getPage(chat_id)==0){
            return;
        }
        String query = " UPDATE Auth\n" +
                "    SET page = ? " +
                "    WHERE chat_id = ?";
        PreparedStatement statement = con.prepareStatement(query);
        statement.setString(2, String.valueOf(chat_id));
        if(direction==Paging.NEXT){
            statement.setInt(1, getPage(chat_id)+1);
        }else{
            statement.setInt(1, getPage(chat_id)-1);
        }
        statement.execute();
    }

    public boolean userAuthenificated(long id) throws SQLException {
        return !"null".equals(getEmail(id));
    }

    public boolean userExists(String email) throws SQLException {
        String query = "select * from users where email = ?";
        PreparedStatement statement = con.prepareStatement(query);
        statement.setString(1, email);
        ResultSet resultSet = statement.executeQuery();
        return resultSet.next();
    }

    public boolean userLoggedIn(String email) throws SQLException {
        String query = "select * from auth where email = ?";
        PreparedStatement statement = con.prepareStatement(query);
        statement.setString(1, email);
        ResultSet resultSet = statement.executeQuery();
        return resultSet.next();
    }

    public void SignUp(String email, long tel_id) throws SQLException {
        String query = " UPDATE users\n" +
                "    SET chat_id = ? " +
                "    WHERE email = ?";
        PreparedStatement statement = con.prepareStatement(query);
        statement.setString(1, String.valueOf(tel_id));
        statement.setString(2, email);
        statement.execute();
    }

    public boolean Verify(String email, String password) throws SQLException {
        String query = "select password from users where email = ?";
        PreparedStatement statement = con.prepareStatement(query);
        statement.setString(1, email);
        ResultSet resultSet = statement.executeQuery();
        resultSet.next();
        String pass = resultSet.getString("password");
        return password.equals(pass);
    }

    public void Exit(long chat_id) throws SQLException {
        con.setAutoCommit(false);
        Savepoint sp = con.setSavepoint("point");
        try {
            String query = " delete from auth where chat_id = ?";
            PreparedStatement statement = con.prepareStatement(query);
            statement.setString(1, String.valueOf(chat_id));
            statement.execute();
            String query2 = " update users set chat_id = null  where chat_id = ?";
            PreparedStatement statement2 = con.prepareStatement(query2);
            statement2.setString(1, String.valueOf(chat_id));
            statement2.execute();
            con.commit();
        } catch (SQLServerException ex) {
            con.rollback(sp);
        }
        con.setAutoCommit(true);
    }
}
