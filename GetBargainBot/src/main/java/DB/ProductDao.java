package DB;


import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;

public class ProductDao {
    Connection con;

    public ProductDao() {
        try {
            con = DBConnection.getConnection();
        } catch (SQLException e) {
            System.out.println("Error getting connection!");
        }
    }

    public ArrayList<String> searchProducts(String name) throws SQLException {
        ArrayList<String> list = new ArrayList<>();
        String query = "SELECT TOP 20 Products.Name as n, Price as pr, M.name as m from Products\n" +
                "join Markets M on Products.MarketId = M.Id\n" +
                "where Products.Name like ?";

        PreparedStatement statement = con.prepareStatement(query);
        statement.setString(1, "%"+name+"%");
        ResultSet resultSet = statement.executeQuery();
        StringBuilder sb;
        while (resultSet.next()) {
            sb = new StringBuilder();
            sb.append(resultSet.getString("n")).append(". Ціна: ");
            sb.append(resultSet.getString("pr")).append(". Заклад: ");
            sb.append(resultSet.getString("m"));
            list.add(sb.toString());
        }
        return list;
    }
}
