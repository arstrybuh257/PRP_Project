package DB;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;


public class DBConnection {
    //private static final String connectionUrl = "jdbc:sqlite:F:\\Java projects\\GetBargainBot\\database.sqlite";
    private static final String connectionUrl = "jdbc:sqlserver://edshelserver.database.windows.net:1433;database=GainBargainDb;user=gainbargaintg@edshelserver;password=VL9ydbmZi6ewDhm;encrypt=true;trustServerCertificate=false;hostNameInCertificate=*.database.windows.net;loginTimeout=30;";
    private static Connection con;

    public static Connection getConnection() throws SQLException {
        if (con == null) {
            con = DriverManager.getConnection(connectionUrl);
        }
        return con;
    }
}
