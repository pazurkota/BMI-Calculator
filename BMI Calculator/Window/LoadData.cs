﻿using System;
using System.Globalization;

namespace BMI_Calculator.Window; 

public class LoadData {
    private static MainWindow _mainWindow;
    private readonly PersonImage _personImage = new(_mainWindow);

    public LoadData(MainWindow mainWindow) {
        _mainWindow = mainWindow;
    }

    /// <summary>
    /// The `LoadDataFromMostRecentUser` method retrieves the name of the most recent user from the application settings.
    /// It then calls the `LoadUsersOnSelectionChanged` method with the retrieved name.
    /// 
    /// The `LoadUsersOnSelectionChanged` method fetches the user data from the database for the given user name. 
    /// If a user with the given name is found, it populates the UI fields with the user's data, including name, gender, age, weight, height, and BMI.
    /// Depending on the user's BMI, it adjusts the UI styling and provides user tips specific to the BMI category (low weight, normal weight, or high weight).
    /// It also changes the displayed image based on the BMI category and the user's gender.
    /// </summary>
    public void LoadDataFromMostRecentUser()
    {
        string name = GSettings.ReadSetting(MainWindow.MostRecentUser);
        if (name != null)
        {
            LoadUsersOnSelectionChanged(name);
        }
    }

    public void LoadUsersOnSelectionChanged(string name)
    {
        UserRepository userRepository = new UserRepository(MainWindow.ConnectionString);
        UserData user = userRepository.GetUserByName(name);

        if (user != null)
        {
            double bmi = Math.Round(user.BMI, 1); ;
            _mainWindow.tb_name.Text = user.Name;
            if (user.Gender == "Male") _mainWindow.cb_male.IsChecked = true;
            if (user.Gender == "Female") _mainWindow.cb_female.IsChecked = true;
            _mainWindow.tb_age.Text = user.Age.ToString();

            // Culture info is used here because for calculations and validations to work i had to settle for . as decimal separator    
            _mainWindow.tb_weight.Text = Math.Round(user.Weight, 1).ToString(CultureInfo.InvariantCulture);
            _mainWindow.tb_height.Text = user.Height.ToString();
            _mainWindow.lbl_result.Content = bmi;
            
            {
                switch (bmi) {
                    case > 18.5 and < 24.9: {
                        _mainWindow.BmiHandler.ChangeLabelBMIScoreStyle(WeightType.Normal);
                    
                        string tip = BMI_Calculator.Window.BmiHandler.GiveTipsForBmi(WeightType.Normal);
                        _mainWindow.lbl_tipscontent.Content = tip;

                        _personImage.SetPersonImage(WeightType.Normal, _mainWindow.IsMale ? 0 : 1);
                        break;
                    }
                    case < 18.5: {
                        _mainWindow.BmiHandler.ChangeLabelBMIScoreStyle(WeightType.Low);
                    
                        string tip = BMI_Calculator.Window.BmiHandler.GiveTipsForBmi(WeightType.Low);
                        _mainWindow.lbl_tipscontent.Content = tip;

                        _personImage.SetPersonImage(WeightType.Low, _mainWindow.IsMale ? 0 : 1);
                        break;
                    }
                    case >= 25: {
                        _mainWindow.BmiHandler.ChangeLabelBMIScoreStyle(WeightType.High);
                    
                        string tip = BMI_Calculator.Window.BmiHandler.GiveTipsForBmi(WeightType.High);
                        _mainWindow.lbl_tipscontent.Content = tip;

                        _personImage.SetPersonImage(WeightType.High, _mainWindow.IsMale ? 0 : 1);

                        break;
                    }
                }
            }
        }
    }
}