using MonefyCloneCourseWork.Model;
using MonefyCloneCourseWork.Service;
using MonefyCloneCourseWork.Service.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MonefyCloneCourseWork.ViewModel
{
    public class CheckViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        DateTime selectedDate = DateTime.Today, cycleStart = DateTime.Today;
        public DateTime SelectedDate { get => selectedDate; set { selectedDate = value; PeriodSelect(); OnPropertyChanged(nameof(SelectedDate)); } } //SelectedDate
        public DateTime CycleStart { get => cycleStart; set { cycleStart = value; PeriodSelect(); OnPropertyChanged(nameof(CycleStart)); } } //for instance if you chooce yearly, it shows 01.01.{year}
        int cyclePeriod = 1;
        string selectedCategory = "Others";
        bool isIncome = false;
        public int CyclePeriod { get => cyclePeriod; set { cyclePeriod = value; PeriodSelect(); OnPropertyChanged(nameof(CyclePeriod)); } } //and 364/365 days
        public string SelectedCategory { get => selectedCategory; set { selectedCategory = value; OnPropertyChanged(nameof(SelectedCategory)); } } //Category of income or expense
        public bool IsIncome { get => isIncome; set { isIncome = value; OnPropertyChanged(nameof(IsIncome)); } } //Income or expense
        double incomeMoney = 0, expenseMoney = 0, selectedMoney = 0;
        public double IncomeMoney { get => incomeMoney; set { incomeMoney = value; OnPropertyChanged(nameof(IncomeMoney)); } }
        public double ExpenseMoney { get => expenseMoney; set { expenseMoney = value; OnPropertyChanged(nameof(ExpenseMoney)); } }
        public double TotalMoney { get { return IncomeMoney - ExpenseMoney; } }
        public double SelectedMoney { get => selectedMoney; set { selectedMoney = Double.Parse(value.ToString("F2")); OnPropertyChanged(nameof(SelectedMoney)); } } //Amount of money got or used
        ObservableCollection<KeyValuePair<string, double>> valueList = new ObservableCollection<KeyValuePair<string, double>>();
        public ObservableCollection<KeyValuePair<string, double>> ValueList
        {
            get => valueList;
            set
            {
                valueList = value;
                OnPropertyChanged(nameof(ValueList));
            }
        }
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        bool IsInValueList(string category)
        {
            foreach (var item in ValueList)
            {
                if(category == item.Key)
                {
                    return true;
                }
            }
            return false;
        }
        
        int ValueListPosition(string category)
        {
            for (int i = 0; i < ValueList.Count; i++)
            {
                if (ValueList[i].Key == category)
                {
                    return i;
                }
            }
            return 0;
        }

        void PeriodSelect()
        {
            ValueList.Clear();
            ExpenseMoney = 0;
            IncomeMoney = 0;
            foreach (var item in Checks)
            {
                if((item.Date - CycleStart).Days < CyclePeriod && (item.Date - CycleStart).Days >= 0)
                {
                    if (!item.IsIncome)
                    {
                        if (IsInValueList(item.Category))
                        {
                            ValueList[ValueListPosition(item.Category)] = new KeyValuePair<string, double>(item.Category, ValueList[ValueListPosition(item.Category)].Value + item.Money);
                        }
                        else
                        {
                            ValueList.Add(new KeyValuePair<string, double>(item.Category, item.Money));
                        }
                        ExpenseMoney += item.Money;
                    }
                    else
                    {
                        IncomeMoney += item.Money;
                    }
                }
            }
            OnPropertyChanged(nameof(ValueList));
            OnPropertyChanged(nameof(TotalMoney));
        }

        private IFileService _fileService;
        public ObservableCollection<Check> Checks { get; set; } = new ObservableCollection<Check>();

        public CheckViewModel(IFileService fileService)
        {
            this._fileService = fileService;
            if (OpenCommand.CanExecute(Checks))
                OpenCommand.Execute(Checks);
        }

        private RelayCommand _saveCommand;
        private RelayCommand _openCommand;
        private RelayCommand _addCommand;
        private RelayCommand _addIncome;
        private RelayCommand _updateCommand;

        public RelayCommand UpdateCategory
        {
            get
            {
                return _updateCommand ?? (_updateCommand = new RelayCommand(Update));
            }
        }

        void Update(object category)
        {
            SelectedCategory = (category.ToString());
        }

        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new RelayCommand(obj =>
                _fileService.Save("checks.json", Checks.Select(c => new Check
                {
                    Money = c.Money,
                    Category = c.Category,
                    IsIncome = c.IsIncome,
                    Date = c.Date
                }).ToList())
                ));
            }
        }

        public RelayCommand OpenCommand
        {
            get
            {
                return _openCommand ?? (_openCommand = new RelayCommand(obj =>
                {
                    var checks = _fileService.Open("checks.json");
                    Checks.Clear();
                    foreach (var c in checks)
                    {
                        Checks.Add(c);
                    }
                    PeriodSelect();
                }
                ));
            }
        }

        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand ?? (_addCommand = new RelayCommand(obj =>
                {
                    Check check = new Check();
                    check.Date = SelectedDate;
                    check.Category = SelectedCategory;
                    check.IsIncome = false;
                    check.Money = SelectedMoney;
                    Checks.Add(check);
                    SelectedCategory = "";
                    SelectedMoney = 0;
                    PeriodSelect();
                    if (SaveCommand.CanExecute(Checks))
                        SaveCommand.Execute(Checks);
                }));
            }
        }

        public RelayCommand AddIncome
        {
            get
            {
                return _addIncome ?? (_addIncome = new RelayCommand(obj =>
                {
                    Check check = new Check();
                    check.Date = SelectedDate;
                    check.Category = SelectedCategory;
                    check.IsIncome = true;
                    check.Money = SelectedMoney;
                    Checks.Add(check);
                    SelectedCategory = "";
                    SelectedMoney = 0;
                    PeriodSelect();
                    if (SaveCommand.CanExecute(Checks))
                        SaveCommand.Execute(Checks);
                }));
            }
        }
    }
}
