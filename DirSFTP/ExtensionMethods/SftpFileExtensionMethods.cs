using Renci.SshNet.Sftp;

namespace GlitchedPolygons.DirSFTP.ExtensionMethods;

public static class SftpFileExtensionMethods
{
    public static string ToOctalNotation(this SftpFile file)
    {
        int ar = Convert.ToInt32(file.OwnerCanRead);
        int aw = Convert.ToInt32(file.OwnerCanWrite);
        int ax = Convert.ToInt32(file.OwnerCanExecute);
        int a = (1 * ax) + (2 * aw) + (4 * ar);

        int gr = Convert.ToInt32(file.GroupCanRead);
        int gw = Convert.ToInt32(file.GroupCanWrite);
        int gx = Convert.ToInt32(file.GroupCanExecute);
        int g = (1 * gx) + (2 * gw) + (4 * gr);

        int or = Convert.ToInt32(file.OthersCanRead);
        int ow = Convert.ToInt32(file.OthersCanWrite);
        int ox = Convert.ToInt32(file.OthersCanExecute);
        int o = (1 * ox) + (2 * ow) + (4 * or);

        return $"{a}{g}{o}";
    }

    public static string ToHumanReadablePermissionMatrix(this SftpFile file)
    {
        return
            $@"
Owner:  
   {(file.OwnerCanRead ? 'R' : '-')}{(file.OwnerCanWrite ? 'W' : '-')}{(file.OwnerCanExecute ? 'X' : '-')}

Group:  
   {(file.GroupCanRead ? 'R' : '-')}{(file.GroupCanWrite ? 'W' : '-')}{(file.GroupCanExecute ? 'X' : '-')}

Others:  
   {(file.OthersCanRead ? 'R' : '-')}{(file.OthersCanWrite ? 'W' : '-')}{(file.OthersCanExecute ? 'X' : '-')}
";
    }
}