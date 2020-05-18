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
        String query = "SELECT o.Name as n, now as pr, MAX(Price) as prp, M.Name as m \n" +
                "FROM [dbo].[Products] o\n" +
                "         LEFT JOIN (\n" +
                "    SELECT x.Name, x.Price AS now\n" +
                "    FROM [dbo].[Products] x\n" +
                "    WHERE x.UploadTime = (\n" +
                "        SELECT MAX(UploadTime)\n" +
                "        FROM [dbo].[Products]\n" +
                "        WHERE Name = x.Name)) q ON q.Name = o.Name\n" +
                "join Categories on Categories.id = o.categoryId\n" +
                "join Markets M on o.MarketId = M.Id\n" +
                "where o.name like ? " +
                "GROUP BY o.Name,M.Name, q.now\n" +
                "HAVING MAX(Price) - now <> 0;";
        PreparedStatement statement = con.prepareStatement(query);
        statement.setString(1, "%"+name+"%");
        ResultSet resultSet = statement.executeQuery();
        StringBuilder sb;
        while (resultSet.next()) {
            sb = new StringBuilder();
            sb.append(resultSet.getString("n")).append(". Нова ціна: ");
            sb.append(resultSet.getString("pr")).append(". Стара ціна: ");
            sb.append(resultSet.getString("prp")).append(". Заклад: ");
            sb.append(resultSet.getString("m"));
            list.add(sb.toString());
            System.out.println(sb.toString());
        }
        return list;
    }
}
