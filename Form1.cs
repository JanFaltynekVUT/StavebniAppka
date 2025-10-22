using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

public class Form1 : Form
{
    public List<Projekt> projekty = new List<Projekt>();  
    public string cestaSouboru = "projekty.txt";          
    public TextBox txtNazev;
    public TextBox txtRozloha;
    public TextBox txtCenaZaM2;
    public ListBox list;
    public Button btnPridat;
    public Button btnSpocitat;
    public Button btnUlozit;
    public Button btnNacist;

    public Form1()
    {
        this.Text = "Stavební kalkulačka (amatérská verze)";
        this.Width = 600;
        this.Height = 400;

        Label lbl1 = new Label();
        lbl1.Text = "Název projektu:";
        lbl1.Left = 10; lbl1.Top = 15;
        this.Controls.Add(lbl1);

        txtNazev = new TextBox();
        txtNazev.Left = 120; txtNazev.Top = 10; txtNazev.Width = 200;
        this.Controls.Add(txtNazev);

        Label lbl2 = new Label();
        lbl2.Text = "Rozloha (m²):";
        lbl2.Left = 10; lbl2.Top = 45;
        this.Controls.Add(lbl2);

        txtRozloha = new TextBox();
        txtRozloha.Left = 120; txtRozloha.Top = 40; txtRozloha.Width = 100;
        this.Controls.Add(txtRozloha);

        Label lbl3 = new Label();
        lbl3.Text = "Cena za m²:";
        lbl3.Left = 10; lbl3.Top = 75;
        this.Controls.Add(lbl3);

        txtCenaZaM2 = new TextBox();
        txtCenaZaM2.Left = 120; txtCenaZaM2.Top = 70; txtCenaZaM2.Width = 100;
        this.Controls.Add(txtCenaZaM2);

        btnPridat = new Button();
        btnPridat.Text = "Přidat projekt";
        btnPridat.Left = 250; btnPridat.Top = 68;
        btnPridat.Click += new EventHandler(BtnPridat_Click);
        this.Controls.Add(btnPridat);

        list = new ListBox();
        list.Left = 10; list.Top = 110; list.Width = 560; list.Height = 180;
        this.Controls.Add(list);

        btnSpocitat = new Button();
        btnSpocitat.Text = "Spočítat vše";
        btnSpocitat.Left = 10; btnSpocitat.Top = 310;
        btnSpocitat.Click += new EventHandler(BtnSpocitat_Click);
        this.Controls.Add(btnSpocitat);

        btnUlozit = new Button();
        btnUlozit.Text = "Uložit";
        btnUlozit.Left = 120; btnUlozit.Top = 310;
        btnUlozit.Click += new EventHandler(BtnUlozit_Click);
        this.Controls.Add(btnUlozit);

        btnNacist = new Button();
        btnNacist.Text = "Načíst";
        btnNacist.Left = 200; btnNacist.Top = 310;
        btnNacist.Click += new EventHandler(BtnNacist_Click);
        this.Controls.Add(btnNacist);
    }

    private void BtnPridat_Click(object sender, EventArgs e)
    {
        Projekt p = new Projekt();
        p.Nazev = txtNazev.Text;
        p.Rozloha = double.Parse(txtRozloha.Text);     
        p.CenaZaM2 = double.Parse(txtCenaZaM2.Text);   
        projekty.Add(p);

        list.Items.Add(p.Nazev + " - " + p.Rozloha + " m² za " + p.CenaZaM2 + " Kč/m²");
        txtNazev.Text = ""; txtRozloha.Text = ""; txtCenaZaM2.Text = "";
    }

    private void BtnSpocitat_Click(object sender, EventArgs e)
    {
        if (projekty.Count == 0)
        {
            MessageBox.Show("Žádné projekty!");
            return;
        }

        double celkemCena = 0;
        double celkemRozloha = 0;
        int i = 0;

        while (i < projekty.Count)
        {
            Projekt p = projekty[i];

            double zaklad = p.Rozloha * p.CenaZaM2;
            double sleva = 0;
            if (p.Rozloha > 100) sleva = 0.05;  
            if (p.Rozloha > 200) sleva = 0.1;   
            double cenaPoSleve = zaklad - (zaklad * sleva);

            double dph = cenaPoSleve * 0.21;
            double konecna = cenaPoSleve + dph;

            double poplatekZaProjekt = (p.Rozloha / 10) * 15; 
            konecna += poplatekZaProjekt;

            celkemCena += konecna;
            celkemRozloha += p.Rozloha;
            i++;
        }

        double prumernaCena = celkemCena / projekty.Count;
        double prumernaRozloha = celkemRozloha / projekty.Count;

        MessageBox.Show(
            "Počet projektů: " + projekty.Count +
            "\nCelková cena (s DPH): " + celkemCena.ToString("N0") + " Kč" +
            "\nPrůměrná rozloha: " + prumernaRozloha.ToString("N2") + " m²" +
            "\nPrůměrná cena projektu: " + prumernaCena.ToString("N0") + " Kč");
    }

    private void BtnUlozit_Click(object sender, EventArgs e)
    {
        StreamWriter w = new StreamWriter(cestaSouboru);
        for (int i = 0; i < projekty.Count; i++)
        {
            Projekt p = projekty[i];
            w.WriteLine(p.Nazev + ";" + p.Rozloha + ";" + p.CenaZaM2);
        }
        w.Close();
        MessageBox.Show("Uloženo");
    }

    private void BtnNacist_Click(object sender, EventArgs e)
    {
        if (!File.Exists(cestaSouboru))
        {
            MessageBox.Show("Soubor neexistuje");
            return;
        }

        projekty.Clear();
        list.Items.Clear();
        StreamReader r = new StreamReader(cestaSouboru);
        while (!r.EndOfStream)
        {
            string line = r.ReadLine();
            string[] d = line.Split(';'); 
            Projekt p = new Projekt();
            p.Nazev = d[0];
            p.Rozloha = double.Parse(d[1]);
            p.CenaZaM2 = double.Parse(d[2]);
            projekty.Add(p);
            list.Items.Add(p.Nazev + " - " + p.Rozloha + " m² za " + p.CenaZaM2 + " Kč/m²");
        }
        r.Close();
        MessageBox.Show("Načteno");
    }

    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.Run(new Form1());
    }
}

// ===== DATOVÁ TŘÍDA =====
public class Projekt
{
    public string Nazev;       
    public double Rozloha;
    public double CenaZaM2;
    
}
