using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Descriptions : MonoBehaviour {

    public static Descriptions descriptions;

    void Awake()
    {
        descriptions = this;
    }

    public Dictionary<string, string> enemyDescs = new Dictionary<string, string>
        {

        #region Floor 1 Enemies

        { "Skeleton", "Deals physical damage.\nSometimes decreases Armor when it attacks.\n\nA reanimated fallen soldier, now only answers to the commander of the dead." },
        { "Spectre", "Deals magic damage.\nSometimes decreases Resolve when it attacks.\n\nThe spirit of what once was a powerful dark mage, brought back to wreak vengeance." },

        { "Zombie", "Deals physical damage.\nSometimes decreases Strength when it attacks\n.Heals itself off damage it deals.\n\nA rotting corpse reanimated by a evil power, now only knows endless hunger." },
        { "Witch", "Buffs other monsters.\nSometimes decreases Intellect when it attacks.\n\nIt may *look* like an undead, but it's not quite there yet."},

        { "Small Acid Slime", "Deals small amounts of physical damage.\nSometimes inflicts Strength and Armor debuffs.\nA tiny blob of acidic slime, it's touch barely stings anymore, it can't get any smaller than this." },
        { "Small Psychic Slime", "Deals small amounts of magic damage.\nSometimes inflicts Intellect and Resolve debuffs.\nA tiny blob of psychic slime that's lost it's antenna, it can't get any smaller than this." },

        { "Acid Slime", "Deals physical damage.\nSometimes inflicts Strength and Armor debuffs.\nCan split into two smaller slimes if it takes too much damage in one turn.\nA glob of acidic slime, its touch can sear your skin." },
        { "Psychic Slime", "Deals magic damage.\nSometimes inflicts Intellect and Resolve debuffs.\nCan split into two smaller slimes if it takes too much damage in one turn.\nA glob of psychic slime, it uses its antenna to telepathically assult the minds of its foes." },

        #endregion

        #region Floor 1 Minibosses

        { "Revenant", "Deals massive physical damage.\nGains Strength when you hit.\nAn undead warrior fueled by endless wrath." },
        { "Wraith", "Deals massive magic damage.\nGains Intellect every turn.\nA powerful dark spirit, slowly kindling its ever-growing power over the millinea." },

        { "Large Acid Slime", "Deals physical damage.\nSometimes inflicts Strength and Armor debuffs.\nCan split into two slimes if it takes too much damage in one turn.\nA massive blob of acidic slime, its touch can melt through the toughest armors." },
        { "Large Psychic Slime", "Deals magic damage.\nSometimes inflicts Intellect and Resolve debuffs.\nCan split into two slimes if it takes too much damage in one turn.\nA massive blob of psychic slime, its telepathy can shatter the strongest of minds." },

        #endregion

        #region Floor 1 Bosses

        { "Necromaster", "Doesn't do the attacking itself, instead summons undead minions to fight for it.\n\nA powerful necromancer, commander of the legions of undead found within this place." },
        { "Mother Slime", "Deals physical and magic damage.\nSomtimes inflicts Strength, Intellect, Armor, and Resovle debuffs.\nCan split into two large slimes if it takes too much damage in one turn.\n\nThe M.O.A.S. - A humongous glob of acidic and psychic slime, the source of all the slimes in this place." }

        #endregion

    };

    public Dictionary<string, string> buffDescs = new Dictionary<string, string>
        {

        #region Stat Buffs

        { "Strength", "Determines physical damage dealt by this unit" },
        { "Intellect", "Determines magic damage dealt and healing done by this unit" },
        { "Armor", "Determines physical damage taken by this unit" },
        { "Resolve", "Determines magic damage taken by this unit" },

        { "Strength Up", "Increases all physical damage this unit deals by [X] ([Y] turns)" },
        { "Intellect Up", "Increases all magic damage this unit deals and healing this unit does by [X] ([Y] turns)" },
        { "Armor Up", "Decreases all physical damage this unit takes by [X] ([Y] turns)" },
        { "Resolve Up", "Decreases all magic damage this unit takes by [X] ([Y] turns)" },

        { "Strength Down", "Decreases all physical damage this unit deals by [X] ([Y] turns)" },
        { "Intellect Down", "Decreases all magic damage this unit deals and healing this unit does by [X] ([Y] turns)" },
        { "Armor Down", "Increases all physical damage this unit takes by [X] ([Y] turns)" },
        { "Resolve Down", "Increases all magic damage this unit takes by [X] ([Y] turns)" },

        { "Shield", "Negates [X] damage this unit takes ([Y] turns)" },
        { "Dodge", "This unit will dodge the next [X] attacks ([Y] turns)" },
        { "Regen", "This unit restores [X] health per turn ([Y] turns)"},

        #endregion

        #region Unique Buffs

        { "Blind", "This unit attacks will always miss ([Y] turns)" },
        { "Daggers", "This unit's next [Y] attacks deal 3 bonus physical damage" },
        { "Poison", "Deals [Y] damage per turn to this unit. Decreases by [Y/10] every turn" },
        { "Burst", "This unit's next AoE attack will deal [Y] bonus magic damage to all enemies" },
        { "Smite", "This unit's attacks deal 5 bonus magic damage ([Y] turns)" },

        { "Freeze", "This unit is frozen and cannot do anything ([Y] turns)" },
        { "Stun", "This unit is stunned and cannot do anything, attacking it will make it snap out of the stun ([Y] turns)" },
        { "Fire", "This unit is on fire and takes 5 damage per turn ([Y] turns)" },
        { "Hellfire", "This unit is on fire and takes 10 damage per tur ([Y] turns)" },

        #endregion

        #region Warrior Buffs

        { "Endless Rage", "This unit is immune to damage and gains Strength every time it is hit" },
        { "Insulting Presence", "This unit taunts and random enemy every turn ([Y] turns)" },
        { "Blood Armor", "When this Shield expires, gain 1 Regen for turns equal to the amount of shield remaining" },

        #endregion

        #region Rogue Buffs

        { "Tripwire", "Attacks against this unit inflict Stun (1 turn) on the attacker ([Y] turns)" },
        { "Bear Trap", "Attacks against this unit will deal 10 physical damage to the attacker ([Y] turns)" },
        { "Blinding Trap", "Attacks against this unit will inflict Blind (1 turn) on the attacker ([Y] turns)" },
        { "Poison Trap", "Attacks against this unit will inflict 3 Poison on the attacker ([Y] turns)" },
        { "Deadly Acrobat", "Every time this unit dodges an attack, it gains 2 daggers ([Y] turns)" },
        { "Sharpen", "This unit's daggers deal 2 additional damage ([Y] turns)" },
        { "Poison Daggers", "This unit's daggers inflict 1 Poison ([Y] turns)" },
        { "Piercing Daggers", "This unit's daggers inflict -1 Armor (1 turn) ([Y] turns)" },
        { "Shadow", "This unit gains 1 Dodge (1 turn) per turn ([Y] turns)" },

        #endregion

        #region Mage Buffs

        { "Aether Barrier", "Any damage to this unit's shield will be gained as Aether ([Y] turns)" },
        { "Aether Barrier (Unleashed)", "Any damage to this unit's shield will be gained as Aether ([Y] turns)" },
        { "Curse", "For every 10 damage this unit takes, the hex will inflict -1 Resolve and -1 Intellect (3 turns) ([Y] turns)" },
        { "Curse (Unleashed)", "For every 10 damage this unit takes, the hex will inflict -2 Resolve and -2 Intellect (4 turns) ([Y] turns)" },

        { "Meditate", "This unit gains +1 Intellect (3 turns) per turn ([Y] turns)" },
        { "Meditate (Unleashed)", "This unit gains +1 Intellect (5 turns) per turn ([Y] turns)" },
        
        { "Fracture", "This unit's next Burst will inflict -2 Armor and -2 Resolve (3 turn) ([Y] turns)" },
        { "Fracture (Unleashed)", "This unit's next Burst will inflict -4 Armor and -4 Resolve (4 turns) ([Y] turns)" },
        { "Flash Bomb", "This unit's next Burst will inflict Stun (2 turns) ([Y] turns)" },
        { "Flash Bomb (Unleashed)", "This unit's next Burst will inflict Blind (2 turns) ([Y] turns)" },

        { "Death Wish", "For every 10 damage this unit takes, the hex will deal 5 magic damage ([Y] turns)" },
        { "Death Wish (Unleashed)", "For every 10 damage this unit takes, the hex will deal 15 magic damage ([Y] turns)" },
        { "Aether Charm", "For every 10 damage this unit takes, the hex will grant the user 5 Aether ([Y] turns)" },
        { "Aether Charm (Unleashed)", "For every 10 damage this unit takes, the hex will grant the user 15 Aether ([Y] turns)" },
        { "Venom Mark", "For every 5 damage this unit takes, the hex will inflict 1 Poison ([Y] turns)" },
        { "Venom Mark (Unleashed)", "For every 5 damage this unit takes, the hex will inflict 3 Poison ([Y] turns)" },

        { "Nova Charge", "This unit gains 3 Burst per turn ([Y] turns)" },
        { "Nova Charge (Unleashed)", "This unit gains 7 Burst per turn ([Y] turns)" },

        #endregion

        #region Cleric Buffs

        { "Regenerative Plating", "This unit gains 5 Shield (1 turn) per turn ([Y] turns)" },
        { "Spiked Shield", "While shielded, enemies that attack this unit take 5 magic damage ([Y] turns)" },
        { "Divine Shield", "While shielded, this unit gains 2 Regen ([Y] turns)" },
        { "Enchanted Shield", "While shielded, this unit gains +3 Armor and +3 Resolve ([Y] turns)" },

        { "Radiance", "This unit's Smites deal an additional 3 magic damage ([Y] turns)" },
        { "Judgement", "This unit's Smites inflict -2 Resolve (2 turns) ([Y] turns)" },
        { "Honor", "This unit's Smites grant 1 Regen (5 turns) ([Y] turns)" },

        #endregion

        #region Enemy Buffs

        { "Undead Rage", "This unit gains Strength every time it is hit" },
        { "Undead Wisdom", "This unit is gaining Intellect every turn" },
        { "Death Mark", "All of the Necromaster's minions are targeting you!" },
        { "Undead Hunger", "This unit heals for 50% of damage it deals"},
        { "Split", "This slime will split into two small slimes if it takes [X] more damage this turn"},
        { "Volatile Split", "This slime will split into two medium slimes if it takes [X] more damage this turn"},
        { "Explosive Split", "This slime will split into two large slimes and inflict a debuff to all players if it takes [X] more damage this turn"},

        #endregion

        { "", "" }

        };

    public Dictionary<string, string> cardDescs = new Dictionary<string, string>
        {

        #region Warrior Cards

        //Warrior Starter Cards
        { "Strike", "Deal 10 physical damage" },
        { "Stand Ground", "Gain 15 Shield (2 turns)" },
        { "Bash", "Deal 10 physical damage, inflict -2 Armor (2 turns)" },
        { "Double Strike", "Deal 6 physical damage twice" },
        { "Strengthen", "Gain +3 Strength (6 turns)" },
        { "Toughen", "Gain +2 Armor and +2 Resolve (6 turns)" },

        //Warrior Beginner Cards
        { "Cleave", "Deal 8 physical damage to all enemies" },
        { "Reckless Strike", "Deal 25 physical damage, gain -5 Armour and -5 Resolve (1 turn)" },
        { "Frenzied Strikes", "Deal 5 physical damage 2-5 times based on your missing health" },
        { "Strengthened Strike", "Deal 15 physical damage, Strength is 3x as effective on this attack" },
        { "Scarred Strength", "Lose 5 health, gain 3 Strength (Endless)" },

        { "Swordbreaker", "Deal 10 physical damage inflict -3 Strength (3 turns)" },
        { "Defensive Strike", "Deal 10 physical damage, gain (5 + damage dealt) Shield (2 turns)" },
        { "Sanguine Strike", "Deal 10 physical damage, heal for 1/2 damage dealt" },
        { "Against the Odds", "Taunt an enemy, then gain 1 Strength for every enemy targeting you (2 turns)" },
        { "Last Stand", "Gain 1-4 Regen based on your missing health (5 turns)" },

        { "Insulting Presence", "Gain passive: Taunt a random enemy every turn (4 turns)" },
        { "Defensive Taunt", "Taunt an enemy and gain 15 Shield (1 turn)" },
        { "Unbreakable", "Gain 10 Shield (Endless) and +1 Armor and +1 Resolve (Endless)" },
        { "Bulwark", "Gain 15 Shield (3 turns), then increase the duration of all your other Shield by 2 turns" },
        { "Blood Armor", "Gain 15 Shield (1 turn), when it expires, gain 1 Regen for turns equal to the amount of remaining Shield" },

        //Warrior Master Cards
        { "Flurry of Blows", "Deal 5 physical damage [X] times, every time you play this card, increase the number of attacks by 1" },
        { "Culling", "Deal 10 physical damage, if this kills an enemy, permanently gain 5 maximum health" },
        { "Barricade", "Gain [X] Shield (1 turn), damage dealt to this shield decreases the power of this card (Min. 15 Shield)" },

        #endregion

        #region Rogue Cards

        //Rogue Starter Cards
        { "Double Slash", "Deal 5 physical damage twice" },
        { "Sneak", "Gain 2 Dodge (2 turns)" },
        { "Preparation", "Gain 5 Daggers" },
        { "Flurry Slash", "Deal 3 physical damage 4 times" },
        { "Tripwire", "Deploy Trap (3 turns): Trap inflicts stun" },
        { "Acidic Poison", "Inflict 4 Poison, -2 Strength, and -2 Armor (2 turns)" },

        //Rogue Beginner Cards
        { "Cloak and Dagger", "Gain 1 Dodge (2 turns) and 3 Daggers" },
        { "Deadly Acrobat", "Gain Passive: Every time you dodge an attack, gain 2 Daggers (Endless)" },
        { "Sharpen", "Daggers deal 2 more damage (Endless)" },
        { "Poison Daggers", "Gain passive: Daggers apply 1 Poison (Endless)" },
        { "Piercing Daggers", "Gain passive: Daggers inflict -1 Armor (1 turn) (Endless)" },

        { "Plague", "Inflict 4 Poison to all enemies" },
        { "Blinding Poison", "Inflict 5 Poison and Blind (1 turn)" },
        { "Paralyzing Poison", "Inflict 5 Poison and Stun (2 turns)" },
        { "Snakebite Slash", "Deal 4 physical damage twice, inflict 2 Poison on each hit" },
        { "Assassin's Technique", "Inflict 4 Poison and gain 2 Dodge (2 turns)" },

        { "Mocking Shadows", "Taunt target enemy and gain 2 Dodge (1 turn)" },
        { "Bear Trap", "Deploy Trap (3 turns): Trap deals 10 physical damage" },
        { "Blinding Trap", "Deploy Trap (3 turns): Trap inflicts Blind (1 turn)" },
        { "Poison Trap", "Deploy Trap (3 turns): Trap inflicts 4 Poison" },
        { "Heads Up", "Grant ally 3 Dodge (1 turn)" },

        //Rogue Master Cards
        { "Infect", "Inflict [X] Poison, after the first time you play this card, it only inflicts 5 Poison" },
        { "Tempest", "Deal 5 physical damage [X] times, every time the last attack of this card kills an enemy, permanently increase the number of attacks by 1" },
        { "Shadow", "Gain 1 Dodge (1 turn) each turn ([X] turns), every time you play this card, its duration is reduced by 1 (Min. 1 turn)" },

        #endregion

        #region Mage Cards

        //Mage Starter Cards
        { "Aether Bolt", "Deal 10 magic damage, gain Aether equal to damage dealt" },
        { "Aether Barrier", "Gain 10 Shield (2 turns), damage dealt to the shield is gained as Aether" },
        { "Curse", "Cast Hex 10 (3 turns): Hex inflicts -1 Resolve and -1 Intellect (3 turns)" },
        { "Arcane Charge", "Gain 7 Burst" },
        { "Enhance Arcana", "Gain +2 Intellect (Endless)" },
        { "Explosion", "Deal 6 magic damage to all enemies" },

        //Unleashed Mage Starter Cards
        { "Aether Bolt (Unleashed)", "Deal 20 magic damage, gain Aether equal to damage dealt" },
        { "Aether Barrier (Unleashed)", "Gain 20 Shield (3 turns), damage dealt to the shield is gained as Aether" },
        { "Curse (Unleashed)", "Cast Hex 10 (3 turns): Hex inflicts -2 Resolve and -2 Intellect (4 turns)" },
        { "Arcane Charge (Unleashed)", "Gain 15 Burst" },
        { "Enhance Arcana (Unleashed)", "Gain +5 Intellect (Endless)" },
        { "Explosion (Unleashed)", "Deal 15 magic damage to all enemies" },

        //Mage Beginner Cards
        { "Magic Missiles", "Deal 5 magic damage 3 times" },
        { "Ancient Knowledge", "Gain (15 + 2x Intellect) Aether" },
        { "Strange Brew", "Gain +6 Intellect (3 turns, decaying) and 3 Poison" },
        { "Meditate", "Gain Passive: Gain +1 Intellect (4 turns) per turn (6 turns)" },
        { "Arcane Armor", "Gain (15 + 2x Intellect) Shield (2 turns)" },

        { "Death Wish", "Cast Hex 10 (3 turns): Hex deals 5 magic damage" },
        { "Aether Charm", "Cast Hex 10 (3 turns): Hex grants you 5 Aether" },
        { "Venom Mark", "Cast Hex 5 (1 turn): Hex inflicts 1 Poison" },
        { "Necrotic Blast", "Deal 25 magic damage, lose 10 health" },
        { "Rend", "Inflict -3 Armor and -3 Resolve (3 turns, decaying) and gain 15 Shield (3 turns, decaying)" },

        { "Aether Reaver", "Deal 5 magic damage to all enemies and gain 3 Aether for each enemy hit" },
        { "Maelstrom", "Deal 4 magic damage to all enemies twice" },
        { "Fracture", "Your next Burst inflicts -2 Armor and -2 Resolve (3 turns)" },
        { "Flash Bomb", "Your next Burst inflicts Stun (2 turns)" },
        { "Blast Shield", "Gain 5 Burst and 10 Shield (1 turn)" },

        //Unleashed Mage Beginner Cards
        { "Magic Missiles (Unleashed)", "Deal 6 magic damage 5 times" },
        { "Ancient Knowledge (Unleashed)", "Gain (30 + 3x Intellect) Aether" },
        { "Strange Brew (Unleashed)", "Gain +10 Intellect (5 turns, decaying)" },
        { "Meditate (Unleashed)", "Gain Passive: Gain +1 Intellect (6 turns) per turn (8 turns)" },
        { "Arcane Armor (Unleashed)", "Gain (30 + 3x Intellect) Shield (2 turns)" },

        { "Death Wish (Unleashed)", "Cast Hex 10 (3 turns): Hex deals 15 magic damage" },
        { "Aether Charm (Unleashed)", "Cast Hex 10 (3 turns): Hex grants you 15 Aether" },
        { "Venom Mark (Unleashed)", "Cast Hex 5 (1 turn): Hex inflicts 3 Poison" },
        { "Necrotic Blast (Unleashed)", "Deal 50 magic damage, lose 5 health" },
        { "Rend (Unleashed)", "Inflict -5 Armor and -5 Resolve (5 turns, decaying) and gain 25 Shield (5 turns, decaying)" },

        { "Aether Reaver (Unleashed)", "Deal 10 magic damage to all enemies and gain 5 Aether for each enemy hit" },
        { "Maelstrom (Unleashed)", "Deal 5 magic damage to all enemies four times" },
        { "Fracture (Unleashed)", "Your next Burst inflicts -4 Armor and -4 Resolve (4 turns)" },
        { "Flash Bomb (Unleashed)", "Your next Burst inflicts Blind (2 turns)" },
        { "Blast Shield (Unleashed)", "Gain 10 Burst and 15 Shield (2 turns)" },

        //Mage Master Cards
        { "Devour Intellect", "Deal 10 magic damage, if this kills an enemy, gain +4 Intellect (Endless)" },
        { "Nova Charge", "Gain 3 Burst per turn ([X] turns), every time you use Burst, increase the duration of this buff by 1" },
        { "Scourge", "Inflict -[X] Armor and -[X] Resolve ([X] turns, decaying), every time this card is played, decrease the power and duration of the debuff by 1 (Minimum of 3)" },

        //Unleashed Mage Master Cards
        { "Devour Intellect (Unleashed)", "Deal 10 magic damage, if this kills an enemy, gain +4 Intellect (Endless) and +1 Intellect (Permanent)" },
        { "Nova Charge (Unleashed)", "Gain 7 Burst per turn ([X] turns), every time you use Burst, increase the duration of this buff by 1" },
        { "Scourge (Unleashed)", "Inflict -[X] Armor and -[X] Resolve ([X] turns), every time this card is played, decrease the power and duration of the debuff by 1 (Minimum of 3)" },

        #endregion

        #region Cleric Cards

        //Cleric Starter Cards
        { "Holy Strike", "Deal 7 physical damage, inflict -1 Armor and -1 Resolve (1 turn)" },
        { "Guard", "Grant 15 Shield (3 turns, decaying)" },
        { "Restore", "Heal an player for 10 health" },
        { "Divinity", "Gain Smite (4 turns)" },
        { "Inspire", "Grant +3 Strength and +3 Intellect (3 turns, decaying)" },
        { "Enfeeble", "Deal 10 magic damage, inflict -2 Strength and -2 Intellect (2 turns)" },

        //Cleric Beginner Cards
        { "Urgent Defense", "Grant player 10-30 Shield based on their missing health (1 turn)" },
        { "Sip of Ichor", "Grant player +3 Strength, Intelligence, Armor, and Resolve (3 turns)" },
        { "Revitalize", "Grant a player 3 Regen (5 turns)" },
        { "Exalt", "Grant player +2 Strength and +2 Intellect (Endless)" },
        { "Healing Prayer", "Heal all players 7 Health" },

        { "Regenerative Plating", "Grant passive to player: Gain 5 Shield (1 turn) per turn (6 turns)" },
        { "Spiked Shield", "Grant passive to player: While shielded, enemies that attack you take 5 magic damage (Endless)" },
        { "Enchanted Shield", "Grant passive: While shielded, gain +2 Armor and +2 Resolve (Endless)" },
        { "Divine Shield", "Grant passive: While shielded, gain 2 Regen (Endless)" },
        { "Perseverance", "Grant all players 10 Shield (2 turns, decaying)" },

        { "Radiance", "Gain passive: Smites deal an additional 3 magic damage (Endless)" },
        { "Judgement", "Gain passive: Smites inflict -2 Armor and -2 Resolve (2 turns) (Endless)" },
        { "Honor", "Gain Passive: Smites grant you 1 Regen (5 turns) (Endless)" },
        { "Divine Protection", "Gain 10 Shield (2 turns), and Smite (3 turns)" },
        { "Twinsmite Strike", "Deal 10 physical damage, apply smite twice" },

        //Cleric Master Cards
        { "Redemption", "Heal an ally for [X] health, every time this heal is used on an ally at or below 15 health, permanently increase the healing by 3" },
        { "Safeguard", "Grant [X] Shield (Endless), every time you play this card, reduce the amount of shield by 5 (Min. 5 Shield)" },
        { "Regicide", "Deal 7 physical damage twice, if the target is the only enemy alive, deal an additional 7 physical damage" },

        #endregion

        { "", "" }

    };

    public Dictionary<string, string> skillDescs = new Dictionary<string, string>
    {
        { "Card Removal I", "Remove a card from your deck" },
        { "Card Removal II", "Remove 2 cards from your deck" },
        { "Card Removal III", "Remove 3 cards from your deck" },

        #region Warrior Skills

        { "Strong", "Permanently gain 1 Strength" },
        { "Fortified", "Start combat with 10 Shield (2 turns)" },
        { "Regenerative", "Heal 5 Health at the start of each combat" },
        { "Hardy", "Permanently gain 10 maximum health" },

        { "Offense", "Add 5 offensive cards to your loot pool" },
        { "Brawler", "Add 5 brawler cards to your loot pool" },
        { "Defense", "Add 5 defensive cards to your loot pool" },

        { "Furyborn", "Gain 0-3 Strength based on your missing health" },
        { "Spiked Armor", "Every time you get hit, the attacker takes 2 physical damage" },
        { "Unarmored Defense", "If you have no Shield at the beginning of the enemies’ turn, gain 5 Shield (1 turn)" },
        { "Taste of Blood", "Every time you kill an enemy, heal 5 health" },

        { "Flurry of Blows", "Gain Master Card:\nDeal 5 physical damage 3 times, every time you play this card, increase the number of attacks by 1" },
        { "Culling", "Gain Master Card:\nDeal 10 physical damage, if this kills an enemy, permanently gain 5 maximum health" },
        { "Barricade", "Gain Master Card:\nGain 60 Shield (1 turn), damage dealt to this shield decreases the power of this card (Min. 15 Shield)" },

        #endregion

        #region Rogue Skills

        { "Prepared", "Start each combat with 3 daggers" },
        { "Poisonous", "At the start of combat, apply 4 Poison to a random enemy" },
        { "Stealthed", "Start each combat with 1 Dodge (2 turns)" },
        { "Alert", "Draw an extra card on your first turn of combat" },

        { "Daggers", "Add 5 dagger cards to your loot pool" },
        { "Poison", "Add 5 poison cards to your loot pool" },
        { "Traps", "Add 5 trap cards to your loot pool" },

        { "Sharp", "Daggers permanently deal 1 extra damage" },
        { "Toxic", "Once per turn, if you apply Poison to an enemy, apply 1 additional poison" },
        { "Cunning", "All traps you deploy have 1 additional duration" },
        { "Focused", "Each round you don’t take damage, gain 2 Focus" },

        { "Infect", "Gain Master Card:\nInflict 10 Poison, after the first time you play this card, it only inflicts 5 Poison" },
        { "Tempest", "Gain Master Card:\nDeal 5 physical damage 3 times, every time the last hit of this card kills an enemy, permanently increase the number of hits by 1" },
        { "Shadow", "Gain Master Card:\nGain 1 Dodge (1 turn) each turn (3 turns), each time this card is played, its duration is increased by 1" },

        #endregion

        #region Mage Skills

        { "Intelligent", "Permanently gain 1 Intellect" },
        { "Shatter", "At the start of combat, inflict a random enemy with -2 Armor and -2 Resolve (3 turns)" },
        { "Explosive", "Start every combat with 4 Burst" },
        { "Arcane Shield", "Start every combat with a 10 Shield Aether Barrier (1 turn)" },

        { "Scaling", "Add 5 scaling cards to your loot pool" },
        { "Hex", "Add 5 hex cards to your loot pool" },
        { "Burst", "Add 5 burst cards to your loot pool" },

        { "Peak Condition", "Gain 0-3 Intellect based on your current health" },
        { "Arcane Burst", "Once per turn, if you gain burst, gain 2 additional Burst" },
        { "Spell Bane", "All hex you apply have 1 additional duration" },
        { "Aether Battery", "Once per turn, if you gain Aether, gain 2 additional Aether" },

        { "Devour Intellect", "Gain Master Card:\nDeal 10 magic damage, if this kills an enemy, gain 4 Intellect (Endless)" },
        { "Scourge", "Gain Master Card:\nInflict -6 Armor and -6 Resolve (6 turns, decaying), every time this card is played, decrease the power and duration of the debuff by 1 (Minimum of 3)" },
        { "Nova Charge", "Gain Master Card:\nGain 3 Burst per turn (4 turns), every time you use Burst, increase the duration of this buff by 1" },

        #endregion

        #region Cleric Skills

        { "Divine Healing", "Heals additionally grant 1 Regen (2 turns)" },
        { "Divine Health", "Permanently gain 10 Max Health" },
        { "Divine Strike", "Start each combat with Smite (3 turns)" },
        { "Divine Vitality", "Permanently gain 1 Regen" },

        { "Supportive", "Add 5 supportive cards to your loot pool" },
        { "Defensive", "Add 5 defensive cards to your loot pool" },
        { "Offensive", "Add 5 offensive cards to your loot pool" },

        { "Protective", "Grant the lowest health ally 10 shield (2 turns) at the start of combat" },
        { "Resolute", "Whenever you Shield a player, grant them +1 Armor and +1 Resolve (1 turn)" },
        { "Radiant", "Smite permanently deals 2 extra damage" },
        { "Divine Aura", "Each turn, the lowest health player is healed 1 Health" },

        { "Redemption", "Gain Master Card:\nHeal an ally for 15 health, every time this heal is used on an ally at or below 15 health, permanently increase the healing by 3" },
        { "Safeguard", "Gain Master Card:\nGrant 20 Shield (Endless), every time you play this card this combat, reduce the amount of Shield by 5 (Minimum 5)" },
        { "Regicide", "Gain Master Card:\nDeal 7 physical damage twice, if the target is the only enemy alive, deal an additional 7 physical damage" },

        #endregion

        { "", "" }

    };

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
