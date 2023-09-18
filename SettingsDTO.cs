using System;
using System.Collections.Generic;
using System.Text;


namespace SimDpsBot
{
    public class SettingsDTO
    {
        public ulong GuildID { get; set; }
        public string Interations { get; set; }
        public string DefaultRealm { get; set; }
        public string World { get; set; }
        public string Fight_Style { get; set; }
        public string Fight_Length { get; set; }
        public string Targets { get; set; }
        public bool Pawn { get; set; }
        public bool SendFileAfterSim { get; set; }
        public bool UseFTP { get; set; }
        public FTPDTO FTP { get; set; }
        public string FTPWebAdress { get; set; }

    }
    public class FTPDTO
    {
        public string Server { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }


    }
}
