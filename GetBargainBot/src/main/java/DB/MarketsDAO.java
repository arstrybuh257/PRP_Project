package DB;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;

public class MarketsDAO {
    Connection con;

    public MarketsDAO() {
        try {
            con = DBConnection.getConnection();
        } catch (SQLException e) {
            System.out.println("Error getting connection!");
        }
    }

    public int getIdByName(String name) throws SQLException {
        ResultSet resultSet;
        String query = "select id from Markets where [name] = ?";
        PreparedStatement statement = con.prepareStatement(query);
        statement.setString(1, String.valueOf(name));
        resultSet = statement.executeQuery();
        if (resultSet.next()) {
            return resultSet.getInt("id");
        } else {
            return -1;
        }
    }
}
