using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace hs_probability {
    public partial class MainForm : Form {
        const int DECK_MAX = 30;
        List<string> deck_lsit = new List<string>();
        List<int> deck = new List<int>();
        List<int> hand = new List<int>();
        Dictionary<int, int> request = new Dictionary<int, int>();
        Dictionary<string, int> card_map = new Dictionary<string, int>(); 
            
        public MainForm() {
            InitializeComponent();
        }

        private void addCard(TextBox count, int card_no) {
            if ("" != count.Text) {
                int c = int.Parse(count.Text);
                for (int i = 0; i < c; i++) {
                    deck.Add(card_no);
                }
            }
        }

        private void addRequest(TextBox card_name, TextBox count) {
            int card_no = card_map[card_name.Text];
            if ("" != count.Text) {
                int c = int.Parse(count.Text);
                for (int i = 0; i < c; i++) {
                    request.Add(card_no, c);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            int card_no = 0;
            textBoxResults.Text = "";

            deck_lsit.Clear();
            card_map.Clear();

            deck_lsit.Add("");
            deck_lsit.Add(textBox1.Text);
            deck_lsit.Add(textBox2.Text);
            deck_lsit.Add(textBox3.Text);
            
            card_no = 1;
            card_map.Add(textBox1.Text, card_no++);
            card_map.Add(textBox2.Text, card_no++);
            card_map.Add(textBox3.Text, card_no++);


            int turncount = int.Parse(textBoxTrunC.Text);

            int handcount = 3;
            if (checkBox1.Checked) {
                handcount = 4;
            }

            // 何回もやってみる
            int[] results = new int[30];

            Random rand = new Random((int)DateTime.Now.Ticks);
            int count = int.Parse(textBoxCount.Text);
            for (int total_count = 0; total_count < count; total_count++) {
                
                hand.Clear();
                deck.Clear();
                card_no = 1;
                addCard(textBoxC1, card_no++);
                addCard(textBoxC2, card_no++);
                addCard(textBoxC3, card_no++);
                if (deck.Count < 30) {
                    int d = (30 - deck.Count);
                    for (int c = 0; c < d; c++) {
                        deck.Add(0);
                    }
                }


                request.Clear();
                addRequest(textBoxH1, textBoxHC1);
                addRequest(textBoxH2, textBoxHC2);
                addRequest(textBoxH3, textBoxHC3);
                addRequest(textBoxH4, textBoxHC4);

                // hand
                for (int h = 0; h < handcount; h++) {
                    int index = rand.Next(deck.Count);
                    var no = deck[index];
                    deck.RemoveAt(index);
                    hand.Add(no);
                }

                //Console.WriteLine(">>");
                // 評価
                foreach (var key in request.Keys.ToList()) {
                    for (int h = 0; h < hand.Count; h++) {
                        if (0 == request[key]) {
                            break;
                        }
                        if (hand[h] == key) {
                            hand[h] = -hand[h]; // 固定
                            request[key] = request[key] - 1;
                            //Console.WriteLine(key);
                        }
                    }
                }

                //Console.WriteLine("--");
                // マリガン
                int m = 0;

                // デッキに戻す
                for (int h = 0; h < hand.Count; h++) {
                    if (hand[h] >= 0) {
                        m++;
                        deck.Add(hand[h]);
                    }
                }
                hand.RemoveAll(x => x >= 0);

                // もう一度引く
                for (int h = 0; h < m; h++) {
                    int index = rand.Next(deck.Count);
                    var no = deck[index];
                    deck.RemoveAt(index);
                    hand.Add(no);
                }
                
                // turn
                for (int h = 0; h < turncount; h++) {
                    int index = rand.Next(deck.Count);
                    var no = deck[index];
                    deck.RemoveAt(index);
                    hand.Add(no);
                }

                
                // 再度評価
                foreach (var key in request.Keys.ToList()) {
                    for (int h = 0; h < hand.Count; h++) {
                        if (hand[h] == key) {
                            hand[h] = -hand[h];
                            if (request[key] > 0) {
                                request[key] = request[key] - 1;
                            }
                        }
                    }
                }


                

                bool hit = true;
                foreach (var key in request.Keys) {
                    if (0 != request[key]) {
                        hit = false;
                    }
                    
                    for (int h = 0; h < hand.Count; h++) {
                        if (0 != hand[h]) {
                            if (Math.Abs(hand[h]) == key) {
                                results[key]++;
                            }
                        }
                    }
                }
                if (hit) {
                    results[0]++;
                }
            }

            string result_text = "";
            result_text += string.Format("{0}/{1} {2}%\r\n", results[0], count, (100 * (double)results[0] / count));
            foreach (var key in request.Keys) {
                result_text += string.Format("{0}:{1}枚\r\n", deck_lsit[key], ((double)results[key] / count));
            }

            textBoxResults.Text = result_text;
        }
    }
}
