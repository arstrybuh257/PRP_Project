package DB;

import java.sql.Connection;
import java.sql.SQLException;

public class SuperCategoryDAO {
    Connection con;

    public SuperCategoryDAO() {
        try {
            con = DBConnection.getConnection();
        } catch (SQLException e) {
            System.out.println("Error getting connection!");
        }
    }

}
