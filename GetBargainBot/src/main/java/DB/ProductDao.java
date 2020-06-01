package DB;


import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

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
        String query = "SELECT TOP 20 ProductsCache.id as id,ProductsCache.Name as n, Price as pr , Prevprice as prpr, M.name as m from ProductsCache\n" +
                "join Markets M on ProductsCache.MarketId = M.Id\n" +
                "where ProductsCache.Name like ?";

        PreparedStatement statement = con.prepareStatement(query);
        statement.setString(1, name + "%");
        ResultSet resultSet = statement.executeQuery();
        StringBuilder sb;
        while (resultSet.next()) {
            boolean hasDiscount = resultSet.getInt("pr") - resultSet.getInt("prpr") != 0;
            sb = new StringBuilder();
            sb.append("[/").append(resultSet.getInt("id")).append("] ");
            sb.append(resultSet.getString("n")).append(". <i>Ціна:</i> ");
            sb.append(resultSet.getString("pr")).append("грн. ");
            if (hasDiscount)
                sb.append("<i>Стара ціна:</i> <s>").append(resultSet.getString("prpr")).append("грн</s>. ");
            sb.append("<i>Заклад:</i> ");
            sb.append(resultSet.getString("m"));
            if (hasDiscount) {
                sb.insert(0, "<u>");
                sb.append("</u>");
            }
            list.add(sb.toString());
        }
        return list;
    }

    public ArrayList<String> searchProducts(int id) throws SQLException {
        ArrayList<String> list = new ArrayList<>();
        String query = "SELECT ProductsCache.id as id,ProductsCache.Name as n, Price as pr , prevprice as prpr, M.name as m from ProductsCache\n" +
                "join Markets M on ProductsCache.MarketId = M.Id\n" +
                " join Categories on Categories.Id = ProductsCache.CategoryId " +
                "where ProductsCache.CategoryId like ?";
        PreparedStatement statement = con.prepareStatement(query);
        statement.setInt(1, id);
        ResultSet resultSet = statement.executeQuery();
        StringBuilder sb;
        while (resultSet.next()) {
            boolean hasDiscount = resultSet.getInt("pr") - resultSet.getInt("prpr") != 0;
            sb = new StringBuilder();
            sb.append("[/").append(resultSet.getInt("id")).append("] ");
            sb.append(resultSet.getString("n")).append(". <i>Ціна:</i> ");
            sb.append(resultSet.getString("pr")).append("грн. ");
            if (hasDiscount)
                sb.append("<i>Стара ціна:</i> <s>").append(resultSet.getString("prpr")).append("грн</s>. ");
            sb.append("<i>Заклад:</i> ");
            sb.append(resultSet.getString("m"));
            if (hasDiscount) {
                sb.insert(0, "<u>");
                sb.append("</u>");
            }
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
        String[] arr = {resultSet.getString("name"), String.valueOf(Math.floor(resultSet.getDouble("price") * 100) / 100), String.valueOf(Math.floor(resultSet.getDouble("prevprice") * 100) / 100), resultSet.getString("imageurl")};
        System.out.println(Arrays.toString(arr));
        return arr;
    }

    public List<String> getFavCategoriesProducts(long tel_id) throws SQLException {
        ArrayList<String> list = new ArrayList<>();
        CategoryDAO dao = new CategoryDAO();
        ResultSet favouriteCategories = dao.getFavouriteCategories(tel_id);
        while (favouriteCategories.next()) {
            String query = "select top 5 ProductsDemo.id as id,ProductsDemo.name as n,price as pr,prevprice as prpr from ProductsDemo\n" +
                    "where CategoryId = ?\n" +
                    "order by (SELECT avg(PrevPrice-Price) as diff from ProductsDemo\n" +
                    "where CategoryId = ?) - (PrevPrice - Price)\n";
            PreparedStatement statement = con.prepareStatement(query);
            statement.setInt(1, favouriteCategories.getInt("CategoryId"));
            statement.setInt(2, favouriteCategories.getInt("CategoryId"));
            ResultSet resultSet = statement.executeQuery();
            StringBuilder sb;
            while (resultSet.next()) {
                sb = new StringBuilder();
                sb.append("[/").append(resultSet.getInt("id")).append("] ");
                sb.append(resultSet.getString("n")).append(". <i>Ціна:</i> ");
                sb.append(resultSet.getString("pr")).append("грн. <i>Стара ціна:</i> <s>");
                sb.append(resultSet.getString("prpr")).append("грн</s>.");
                list.add(sb.toString());
            }
        }
        return list;
    }

    public List<String> getFavProducts(long tel_id) throws SQLException {
        ArrayList<String> list = new ArrayList<>();
        String query = "select ProductsCache.Id as id ,ProductsCache.Name as name,ProductsCache.Price as pr,ProductsCache.PrevPrice as prpr,M.Name as m from FavoriteProducts\n" +
                "join Users U on FavoriteProducts.UserId = U.Id\n" +
                "join ProductsCache on FavoriteProducts.ProductId = ProductsCache.Id\n" +
                "join Markets M on ProductsCache.MarketId = M.Id\n" +
                "where chat_id = ?";
        PreparedStatement p = con.prepareStatement(query);
        p.setString(1, String.valueOf(tel_id));
        ResultSet resultSet = p.executeQuery();
        StringBuilder sb;
        while (resultSet.next()) {
            boolean hasDiscount = resultSet.getInt("pr") - resultSet.getInt("prpr") != 0;
            sb = new StringBuilder();
            sb.append("[/").append(resultSet.getInt("id")).append("] ");
            sb.append(resultSet.getString("name")).append(". <i>Ціна:</i> ");
            sb.append(resultSet.getString("pr")).append("грн. ");
            if (hasDiscount)
                sb.append("<i>Стара ціна:</i> <s>").append(resultSet.getString("prpr")).append("грн</s>. ");
            sb.append("<i>Заклад:</i> ");
            sb.append(resultSet.getString("m"));
            if (hasDiscount) {
                sb.insert(0, "<u>");
                sb.append("</u>");
            }
            list.add(sb.toString());
        }

        return list;
    }

    public boolean addToFav(long tel_id, long item_id)  {
        try {
            String query2 = "select id from users " +
                    "where chat_id=?";
            PreparedStatement statement2 = con.prepareStatement(query2);
            statement2.setString(1, String.valueOf(tel_id));
            ResultSet resultSet = statement2.executeQuery();
            resultSet.next();
            String query = " INSERT into FavoriteProducts(productId,userId) values(?,?)";
            PreparedStatement statement = con.prepareStatement(query);
            statement.setInt(1, (int)item_id);
            statement.setInt(2, resultSet.getInt("id"));
            statement.execute();
        } catch (SQLException ex) {
            System.out.println(ex.toString());
            return false;
        }
        return true;
    }

    public boolean isFavorite(String id) throws SQLException {
            String query = "select productId from favoriteProducts " +
                    "where productId=?";
            PreparedStatement statement = con.prepareStatement(query);
            statement.setString(1, String.valueOf(id));
            ResultSet resultSet = statement.executeQuery();
            return resultSet.next();

    }

}



