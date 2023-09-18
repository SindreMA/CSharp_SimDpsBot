using System;
using System.Collections.Generic;
using System.Text;

namespace SimDpsBot
{
    public class WowCharacter
    {
        public string lastModified { get; set; }
        public string name { get; set; }
        public string realm { get; set; }
        public string battlegroup { get; set; }
        public string Class { get; set; }
        public string gender { get; set; }
        public string level { get; set; }
        public string achievementPoints { get; set; }
        public string thumbnail { get; set; }
        public string calcClass { get; set; }
        public string faction { get; set; }
        public string totalHonorableKills { get; set; }
    }
    public class WowTalentCharacter
    {
        public long lastModified { get; set; }
        public string name { get; set; }
        public string realm { get; set; }
        public string battlegroup { get; set; }
        public int _class { get; set; }
        public int race { get; set; }
        public int gender { get; set; }
        public int level { get; set; }
        public int achievementPoints { get; set; }
        public string thumbnail { get; set; }
        public string calcClass { get; set; }
        public int faction { get; set; }
        public Talent[] talents { get; set; }
        public int totalHonorableKills { get; set; }
    }

    public class Talent
    {
        public bool selected { get; set; }
        public Talent1[] talents { get; set; }
        public Spec spec { get; set; }
        public string calcTalent { get; set; }
        public string calcSpec { get; set; }
    }

    public class Spec
    {
        public string name { get; set; }
        public string role { get; set; }
        public string backgroundImage { get; set; }
        public string icon { get; set; }
        public string description { get; set; }
        public int order { get; set; }
    }

    public class Talent1
    {
        public int tier { get; set; }
        public int column { get; set; }
        public Spell spell { get; set; }
        public Spec1 spec { get; set; }
    }

    public class Spell
    {
        public int id { get; set; }
        public string name { get; set; }
        public string icon { get; set; }
        public string description { get; set; }
        public string castTime { get; set; }
        public string range { get; set; }
        public string powerCost { get; set; }
        public string cooldown { get; set; }
    }

    public class Spec1
    {
        public string name { get; set; }
        public string role { get; set; }
        public string backgroundImage { get; set; }
        public string icon { get; set; }
        public string description { get; set; }
        public int order { get; set; }
    }

}
