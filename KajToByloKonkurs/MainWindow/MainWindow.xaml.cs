﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Collections.ObjectModel;

namespace KajToBylo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum IndexCategory { MusicPL, MusicSL, Movie, Book };
        public enum CorrectAnswer { AnswerA, AnswerB, AnswerC, AnswerD };
        public static string[] NameCategory = { "Muzyka Polska", "Muzyka Śląska", "Film", "Książka" };

        private Base myBase;
        private Collections collections;

        public MainWindow()
        {
            collections = new Collections();
            InitializeComponent();
            tabControl.DataContext = collections;
        }

        private void newBase_Click(object sender, RoutedEventArgs e)
        {
            if (myBase != null)
                saveBaseAndClear();
            
            DialogNewBase newBase = new DialogNewBase();
            newBase.ShowDialog();

            if (newBase.DialogNewBaseResult)
            {
                myBase = new Base(newBase.NameBase);

                enabledControls();
            }

            newBase.Close();
        }

        private void newQuestion_Click(object sender, RoutedEventArgs e)
        {
            DialogNewQuestion newQuestion = new DialogNewQuestion();
            newQuestion.ShowDialog();

            if (newQuestion.DialogNewQuestionResult)
            {
                myBase.SetCategory(newQuestion.Category, newQuestion.Question);
                collections.AddItemsToCollections(newQuestion.Category, newQuestion.Question);

                RefreshListsView(newQuestion.Category);

            }
            newQuestion.Close();
        }

        private void RefreshListsView(IndexCategory indexCategory)
        {
            switch (indexCategory)
            {
                case IndexCategory.MusicPL:
                    listViewMusicPL.Items.Refresh();
                    break;
                case IndexCategory.MusicSL:
                    listViewMusicSL.Items.Refresh();
                    break;
                case IndexCategory.Movie:
                    listViewMovie.Items.Refresh();
                    break;
                case IndexCategory.Book:
                    listViewBook.Items.Refresh();
                    break;
            }
        }
        private void openBase_Click(object sender, RoutedEventArgs e)
        {
            if (myBase != null)
                saveBaseAndClear();

            FolderBrowserDialog openBase = new FolderBrowserDialog();
            openBase.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\KajToBylo\";

            DialogResult result = openBase.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                int index = openBase.SelectedPath.LastIndexOf("\\");
                myBase = new Base(openBase.SelectedPath.Substring(index + 1));
                myBase.ReadALL(openBase.SelectedPath);

                SetCollections();

                enabledControls();
            }
        }

        private void saveBaseAndClear()
        {
            DialogSaveBase saveBase = new DialogSaveBase();
            saveBase.ShowDialog();

            if (saveBase.DialogSaveBaseResult)
            {
                myBase.WriteAll();
                
            }
            else
                collections.Clear();

            saveBase.Close();
        }

        private void saveBase_Click(object sender, RoutedEventArgs e)
        {
            myBase.WriteAll();
        }

        private void closeApp_Click(object sender, RoutedEventArgs e)
        {
            if (myBase != null)
                saveBaseAndClear();
            
            Environment.Exit(0);
        }

        private void SetCollections()
        {
            for (int i = 0; i < NameCategory.Count(); i++)
            {
                collections.SetCollections((IndexCategory)i, myBase.GetCategory((IndexCategory)i));
            }
        }

        private void enabledControls()
        {
            buttonSaveBase.IsEnabled = true;
            buttonNewQuestion.IsEnabled = true;

            menuSaveBase.IsEnabled = true;
            menuAddQuestion.IsEnabled = true;
            menuDeleteQuestion.IsEnabled = true;
        }


        private void getSelectedItem(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.ListView listView = sender as System.Windows.Controls.ListView;

            QuestionAnswers question = (QuestionAnswers)listView.SelectedItems[0];
            if (question.Used == true)
                question.Used = false;
            else
                question.Used = true;
            listView.Items.Refresh();
            //myBase.Used(IndexCategory.MusicPL, question);

            //System.Windows.MessageBox.Show(question.ToString());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (myBase != null)
                saveBaseAndClear();
            
            Environment.Exit(0);
        }

        private void buttonDeleteQuestion_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button buttonDeleteQuestion = sender as System.Windows.Controls.Button;
            QuestionAnswers question = buttonDeleteQuestion.DataContext as QuestionAnswers;

            collections.DeleteItem(CheckCategory(buttonDeleteQuestion.Name), question);
            myBase.DeleteQuestion(CheckCategory(buttonDeleteQuestion.Name), question);
            RefreshListsView(CheckCategory(buttonDeleteQuestion.Name));
        }

        private IndexCategory CheckCategory(string button)
        {
            if (button == "buttonDeleteMusicPL")
                    return IndexCategory.MusicPL;
            else if(button == "buttonDeleteMusicSL")
                    return IndexCategory.MusicSL;
            else if (button == "buttonDeleteMovie")
                return IndexCategory.Movie;
            else
                return IndexCategory.Book;
        }
        
    }
}

