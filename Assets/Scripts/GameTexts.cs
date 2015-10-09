using UnityEngine;
using System.Collections;

class GameTexts
{


	public static GameTexts instance = new GameTexts();
	// ============

	// ===== MAIN MENU =====

	public string[] mainMenu_introTexts = {

		// [0]
		"Morse. A simple sound based encoding got to dominate the world.\n" +
		"As always, the first to say the new word were the guns. Even Hell\n" +
		"heard. Then - computers, networks, automatons. Today - everything,\n" +
		"everyone - crib to grave.",

		// [1]
		"People. Hearts, beating in dits and dahs. Desiring to integrate\n" +
		"even deeper, cutting throats to insert Transmitters. Disgorging\n" +
		"Morse right from inside their bodies to talk directly to tech,\n" +
		"to people, to the world. To the new life.",

		// [2]
		"Irvin. A man below ordinary. He begs the tears to flow on his\n" +
		"brother's grave. Yet there's no dit-dah for that. Undampened, it\n" +
		"is all razor-sharp: the one last piece of his shattered family\n" +
		"and life is some box brother left him at Hackney's PD. The Box."

	};


	// ===== WIN MENU =====
	public string winMenu_mainText = 
		"You have WON this demo?!\n" +
		"Oh, awesome YOU! ^_^\n" +
		"\n\n\n\n\n\n\n\n\n\n\n\n\n" +
		"More MORSE is probable,\n" +
		"if you liked it. Tell us:";

	public string winMenu_twAB = "@Arvydas_B";
	public string winMenu_twAP = "@aprotasenya";


	// ===== LEVEL 1 (demo level) =====

	public string[] level1_ditMessages = {
		"--- message from: DIT ---\n" +
		"Oi! You.\n" +
		"See that mr. Shiny?\n" +
		"\n" +
		"Get close, but stay back.\n" +
		"Morse \'L\' - (un)LOCK it\n" +
		"Morse \'I\' - read it\'s INFO"
		,

		"--- message from: DIT ---\n" +
		"Well done, matey.\n" +
		"morse 'K' - KILL it\n" +
		"\n" +
		"Hey! No doubts.\n" +
		"It would SURE kill you.\n" +
		"Cheers."
		
	};

	public string[] level1_robotMemory = {


		"A robot can contain serious amount of data",

		"Though, not too big, it's not a memory bank",

		"You know, a decent robot's amount - enough.",

		"1_/_ct/__5_: A X4 police automaton was used in an" +
		"encounter with an aggressive subject (Impulse Morse" +
		"limited usage allowed). Subject terminated.",

		"21/J_n/1__7: Damn, Jason! I’m saying it the last" +
		"time: you use E-Morse terminal for personal needs -" +
		"I’ll kick your fat ass out of here!",

		"_3/D_c/__60: M hackers are 50% more active this month." +
		"Bastards are looking for smth in the M-net. I've sent" +
		"one more request higher. No answer yet.",

		"15/De_/1_6_: Batch deleted: personal file 33456;" +
		"personal file 33457. Personal file 33458 changed:" +
		"$notes = “Eat this, you morons! M-Hakzz out!”"

	};
	
}
