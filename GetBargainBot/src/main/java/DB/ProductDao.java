package DB;


import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.Arrays;

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
        String query = "SELECT TOP 20 ProductsCache.id as id,ProductsCache.Name as n, Price as pr, M.name as m from ProductsCache\n" +
                "join Markets M on ProductsCache.MarketId = M.Id\n" +
                "where ProductsCache.Name like ?";

        PreparedStatement statement = con.prepareStatement(query);
        statement.setString(1, "%" + name + "%");
        ResultSet resultSet = statement.executeQuery();
        StringBuilder sb;
        while (resultSet.next()) {
            sb = new StringBuilder();
            sb.append("[/").append(resultSet.getInt("id")).append("] ");
            sb.append(resultSet.getString("n")).append(". Ціна: ");
            sb.append(resultSet.getString("pr")).append(". Заклад: ");
            sb.append(resultSet.getString("m"));
            list.add(sb.toString());
        }
        return list;
    }

    public ArrayList<String> searchProducts(int id) throws SQLException {
        ArrayList<String> list = new ArrayList<>();
        String query = "SELECT ProductsCache.id as id,ProductsCache.Name as n, Price as pr, M.name as m from ProductsCache\n" +
                "join Markets M on ProductsCache.MarketId = M.Id\n" +
                " join Categories on Categories.Id = ProductsCache.CategoryId " +
                "where ProductsCache.CategoryId like ?";
        PreparedStatement statement = con.prepareStatement(query);
        statement.setInt(1, id);
        ResultSet resultSet = statement.executeQuery();
        StringBuilder sb;
        while (resultSet.next()) {
            sb = new StringBuilder();
            sb.append("[/").append(resultSet.getInt("id")).append("] ");
            sb.append(resultSet.getString("n")).append(". Ціна: ");
            sb.append(resultSet.getString("pr")).append(". Заклад: ");
            sb.append(resultSet.getString("m"));
            list.add(sb.toString());
        }
        return list;
    }

    public String[] getProductById(String id) throws SQLException {
        String query = "SELECT name,price,prevprice,imageurl from productscache where id=?";
        PreparedStatement statement = con.prepareStatement(query);
        statement.setInt(1, Integer.parseInt(id));
        ResultSet resultSet = statement.executeQuery();
        resultSet.next();
        String[] arr = {resultSet.getString("name"),String.valueOf(Math.floor(resultSet.getDouble("price") * 100) / 100),String.valueOf(Math.floor(resultSet.getDouble("prevprice") * 100) / 100),resultSet.getString("imageurl")};
        System.out.println(Arrays.toString(arr));
        return arr;
    }
}



