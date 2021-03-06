﻿using Microsoft.Win32;
using OxyPlot.Series;
using Pickaxe.AlgorithmFramework;
using Pickaxe.Model;
using Pickaxe.Utility;
using Pickaxe.Utility.ListExtension;
using Pickaxe.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Pickaxe.ViewModel
{
    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        private Relation _relation;
        private string _fileName;
        private int _binNumber;

        private int _histogramBinNumber;
        private ObservableCollection<ColumnItem> _histogramBins;

        private AlgorithmDiscovery _algorithmDiscovery;

        private ICommand _newRelation;
        private ICommand _openRelation;
        private ICommand _reloadRelation;
        private ICommand _saveRelation;
        private ICommand _saveAsRelation;

        private ICommand _loadRelationFromCSV;
        private ICommand _loadRelationFromSerialized;

        private ICommand _addAttribute;
        private ICommand _replaceAttribute;
        private ICommand _insertAttribute;
        private ICommand _removeAttribute;

        private ICommand _refreshStatisticsView;

        private ICommand _runAlgorithm;

        private ObservableCollection<AlgorithmHistoryViewModel> _clusterAlgorithmHistoryCollection;
        private ObservableCollection<AlgorithmHistoryViewModel> _classifyAlgorithmHistoryCollection;
        private ObservableCollection<AlgorithmHistoryViewModel> _associateAlgorithmHistoryCollection;

        public int BinNumber
        {
            get => _binNumber;
            set
            {
                _binNumber = value;
                OnPropertyChanged("BinNumber");
            }
        }

        public Relation Relation
        {
            get => _relation;
            set
            {
                _relation = value;
                OnPropertyChanged("Relation");
            }
        }

        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                OnPropertyChanged("FileName");
            }
        }

        public int HistogramBinNumber
        {
            get => _histogramBinNumber;
            set
            {
                _histogramBinNumber = value;
                OnPropertyChanged("HistogramBinNumber");
            }
        }

        public ObservableCollection<ColumnItem> HistogramBins
        {
            get => _histogramBins;
            set
            {
                _histogramBins = value;
                OnPropertyChanged("HistogramBins");
            }
        }

        public AlgorithmDiscovery AlgorithmDiscovery
        {
            get => _algorithmDiscovery;
            set
            {
                _algorithmDiscovery = value;
                OnPropertyChanged("AlgorithmDiscovery");
            }
        }

        public ObservableCollection<AlgorithmHistoryViewModel> ClusterAlgorithmHistoryCollection
        {
            get => _clusterAlgorithmHistoryCollection;
            set
            {
                _clusterAlgorithmHistoryCollection = value;
                OnPropertyChanged("ClusterAlgorithmHistoryCollection");
            }
        }

        public ObservableCollection<AlgorithmHistoryViewModel> ClassifyAlgorithmHistoryCollection
        {
            get => _classifyAlgorithmHistoryCollection;
            set
            {
                _classifyAlgorithmHistoryCollection = value;
                OnPropertyChanged("ClusterAlgorithmHistoryCollection");
            }
        }

        public ObservableCollection<AlgorithmHistoryViewModel> AssociateAlgorithmHistoryCollection
        {
            get => _associateAlgorithmHistoryCollection;
            set
            {
                _associateAlgorithmHistoryCollection = value;
                OnPropertyChanged("ClusterAlgorithmHistoryCollection");
            }
        }

        public MainWindowViewModel()
        {
            Relation = new Relation();
            FileName = null;
            HistogramBinNumber = 10;
            HistogramBins = new ObservableCollection<ColumnItem>();
            AlgorithmDiscovery = new AlgorithmDiscovery();
            ClusterAlgorithmHistoryCollection = new ObservableCollection<AlgorithmHistoryViewModel>();
            ClassifyAlgorithmHistoryCollection = new ObservableCollection<AlgorithmHistoryViewModel>();
            AssociateAlgorithmHistoryCollection = new ObservableCollection<AlgorithmHistoryViewModel>();
        }

        public ICommand NewRelation
        {
            get => _newRelation ?? (
                _newRelation = new RelayCommand(
                    parameter => Relation.Count != 0,
                    parameter =>
                    {
                        Relation = new Relation();
                        FileName = null;
                    })
                );
        }

        public ICommand OpenRelation
        {
            get => _openRelation ?? (
                _openRelation = new RelayCommand(
                    parameter => true,
                    parameter =>
                    {
                        var openFileDialog = new OpenFileDialog
                        {
                            Filter = FileFilter
                        };
                        if (openFileDialog.ShowDialog() == true)
                        {
                            using (var stream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                            {
                                Relation relation = Formatter.Deserialize(stream);
                                Relation = relation;
                                FileName = openFileDialog.FileName;
                                //try
                                //{
                                //    Relation relation = Formatter.Deserialize(stream);
                                //    Relation = relation;
                                //    FileName = openFileDialog.FileName;
                                //}
                                //catch (Exception)
                                //{
                                //    MessageBox.Show("Selected file is corrupted, please select another file",
                                //        "Invalid Pickaxe file");
                                //}
                            }
                        }
                        // Do nothing
                    })
                );
        }

        public ICommand ReloadRelation
        {
            get => _reloadRelation ?? (
                _reloadRelation = new RelayCommand(
                    parameter => FileName != null,
                    parameter =>
                    {
                        using (var stream = new FileStream(FileName, FileMode.Open, FileAccess.Read))
                        {
                            Relation = Formatter.Deserialize(stream);
                        }
                    })
                );
        }

        public ICommand SaveRelation
        {
            get => _saveRelation ?? (
                _saveRelation = new RelayCommand(
                    parameter => true,
                    parameter =>
                    {
                        if (FileName == null)
                        {
                            var saveFileDialog = new SaveFileDialog
                            {
                                Filter = FileFilter
                            };
                            if (saveFileDialog.ShowDialog() == true)
                                FileName = saveFileDialog.FileName;
                            else
                                return; // Do nothing
                        }
                        using (var stream = new FileStream(FileName, FileMode.Create, FileAccess.Write))
                        {
                            Formatter.Serialize(stream, Relation);
                        }
                    })
                );
        }

        public ICommand SaveAsRelation
        {
            get => _saveAsRelation ?? (
                _saveAsRelation = new RelayCommand(
                    parameter => true,
                    parameter =>
                    {
                        var saveFileDialog = new SaveFileDialog
                        {
                            Filter = FileFilter
                        };
                        if (saveFileDialog.ShowDialog() == true)
                        {
                            FileName = saveFileDialog.FileName;
                            using (var stream = new FileStream(FileName, FileMode.Create, FileAccess.Write))
                            {
                                Formatter.Serialize(stream, Relation);
                            }
                        }
                    })
                );
        }

        public ICommand LoadRelationFromCSV
        {
            get => _loadRelationFromCSV ?? (_loadRelationFromCSV = new RelayCommand(
                    parameter => true,
                    parameter =>
                    {
                        var openFileDialog = new OpenFileDialog
                        {
                            Filter = CSVFileFilter
                        };
                        if (openFileDialog.ShowDialog() == true)
                        {
                            using (var stream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                            {
                                try
                                {
                                    Relation = CSVFormatter.Deserialize(stream);
                                    FileName = null;
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show("Selected file is corrupted, please select another file",
                                        "Invalid CSV file");
                                }
                            }
                        }
                        // Do nothing
                    }
                ));
        }

        public ICommand LoadRelationFromSerialized
        {
            get => _loadRelationFromSerialized ?? (_loadRelationFromSerialized = new RelayCommand(
                    parameter => true,
                    parameter =>
                    {
                        var openFileDialog = new OpenFileDialog
                        {
                            Filter = FileFilter
                        };
                        if (openFileDialog.ShowDialog() == true)
                        {
                            using (var stream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                            {
                                try
                                {
                                    Relation = BinaryFormatter.Deserialize(stream);
                                    FileName = null;
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show("Selected file is corrupted, please select another file",
                                        "Invalid Pickaxe Serialized file");
                                }
                            }
                        }
                        // Do nothing
                    }
                ));
        }

        public ICommand AddAttribute
        {
            get => _addAttribute ?? (
                _addAttribute = new RelayCommand(
                    parameter =>
                    {
                        return Relation != null;
                    },
                    parameter =>
                    {
                        var attribute = ShowDialogForNewAttribute();
                        if (attribute != null)
                        {
                            Relation.Add(attribute);
                        }
                    })
                );
        }

        public ICommand ReplaceAttribute
        {
            get => _replaceAttribute ?? (
                _replaceAttribute = new RelayCommand(
                    parameter =>
                    {
                        if (Relation == null)
                            return false;
                        if (parameter is int inInt)
                        {
                            return inInt != -1;
                        }
                        else
                        {
                            return false;
                        }
                    },
                    parameter =>
                    {
                        var index = (int)parameter;
                        var attribute = Relation[index];
                        var newAttribute = ShowDialogForReplaceAttribute(attribute);
                        if (newAttribute == null)
                            return;
                        foreach (var v in attribute.Data)
                        {
                            if (!newAttribute.Type.ValidateValueWithMissing(v))
                            {
                                MessageBox.Show("New attribute is not compatible to current data", "Replace Failed");
                                return;
                            }
                        }
                        newAttribute.Data = attribute.Data;
                        Relation[index] = newAttribute;
                    })
                );
        }

        public ICommand InsertAttribute
        {
            get => _insertAttribute ?? (
                _insertAttribute = new RelayCommand(
                    parameter =>
                    {
                        if (Relation == null)
                            return false;
                        if (parameter is int inInt)
                        {
                            return inInt != -1;
                        }
                        else
                        {
                            return false;
                        }
                    },
                    parameter =>
                    {
                        var attribute = ShowDialogForNewAttribute();
                        if (attribute != null)
                        {
                            Relation.Insert((int)parameter, attribute);
                        }
                    })
                );
        }

        public ICommand RemoveAttribute
        {
            get => _removeAttribute ?? (
                _removeAttribute = new RelayCommand(
                    parameter =>
                    {
                        if (Relation == null)
                            return false;
                        if (parameter == null)
                            return false;
                        return (int)parameter < Relation.Count;
                    },
                    parameter =>
                    {
                        Relation.RemoveAt((int)parameter);
                    })
                );
        }

        public ICommand RefreshStatisticsView
        {
            get => _refreshStatisticsView ?? (
                _refreshStatisticsView = new RelayCommand(
                    parameter =>
                    {
                        var attribute = (RelationAttribute)parameter;
                        if (Relation == null)
                            return false;
                        if (parameter == null)
                            return false;
                        return true;
                    },
                    parameter =>
                    {
                        var attribute = (RelationAttribute)parameter;
                        attribute.StatisticView.Refresh();

                        var bins = new List<ColumnItem>();
                        for (int i = 0; i < HistogramBinNumber; ++i)
                            bins.Add(new ColumnItem(0));

                        var max = attribute.StatisticView.Max;
                        var min = attribute.StatisticView.Min;

                        if (!max.IsMissing() && !min.IsMissing())
                        {
                            var binSize = (max - min) / HistogramBinNumber;
                            foreach (var value in attribute.Data)
                            {
                                if (!value.IsMissing())
                                {
                                    int bin;
                                    if (value != max)
                                    {
                                        bin = (int)Math.Floor((value - min) / binSize);
                                    }
                                    else
                                    {
                                        bin = HistogramBinNumber - 1;
                                    }
                                    bins[bin].Value += 1;
                                }
                            }
                        }
                        HistogramBins = new ObservableCollection<ColumnItem>(bins);
                    })
                );
        }

        public ICommand RunAlgorithm
        {
            get => _runAlgorithm ?? (
                _runAlgorithm = new RelayCommand(
                    parameter =>
                    {
                        if (Relation == null)
                            return false;
                        if (parameter == null)
                            return false;
                        var parameters = (object[])parameter;
                        var algorithm = (IAlgorithm)parameters[0];
                        if (algorithm == null)
                            return false;
                        return true;
                    },
                    parameter =>
                    {
                        var parameters = (object[])parameter;
                        var algorithm = (IAlgorithm)parameters[0];
                        var output = (TextBox)parameters[1];
                        algorithm.Output = output;
                        algorithm.Relation = Relation;

                        var dialog = new OptionDialog();
                        dialog.ViewModel.Relation = Relation;
                        dialog.ViewModel.Name = algorithm.Name;
                        dialog.ViewModel.Description = algorithm.Description;
                        dialog.ViewModel.Options = algorithm.Options;
                        if (dialog.ShowDialog() == true)
                        {
                            output.Clear();
                            algorithm.Run();
                        // Add history
                        var history = new AlgorithmHistoryViewModel
                            {
                                Name = algorithm.Name,
                                DateTime = DateTime.Now,
                                OutputText = output.Text,
                            };
                            switch (algorithm.Type)
                            {
                                case AlgorithmType.Cluster:
                                    ClusterAlgorithmHistoryCollection.Add(history);
                                    break;
                                case AlgorithmType.Classify:
                                    ClassifyAlgorithmHistoryCollection.Add(history);
                                    break;
                                case AlgorithmType.Associate:
                                    AssociateAlgorithmHistoryCollection.Add(history);
                                    break;
                                default:
                                    break; // Do not add history
                        }
                        }
                    })
                );
        }

        #region Static members

        private const string FileFilter = "Pickaxe files (*.pickaxe)|*.pickaxe|All files (*.*)|*.*";
        private const string CSVFileFilter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
        private static readonly IRelationFormatter Formatter = new RelationFormatter();
        private static readonly IRelationFormatter BinaryFormatter = new BinaryRelationFormatter();
        private static readonly IRelationFormatter CSVFormatter = new CSVRelationFormatter();

        #endregion

        #region Methods

        private RelationAttribute ShowDialogForNewAttribute()
        {
            var dialog = new AttributeEditDialog();
            if (dialog.ShowDialog() == true)
            {
                var data = new ObservableCollection<Value>();
                data.Resize(Relation.TuplesView.Count, Value.MISSING);
                return new RelationAttribute(dialog.ViewModel.Name, dialog.ViewModel.AttributeType, data);
            }
            else
            {
                return null;
            }
        }

        private RelationAttribute ShowDialogForReplaceAttribute(RelationAttribute attribute)
        {
            var dialog = new AttributeEditDialog();
            dialog.ViewModel.Name = attribute.Name;
            if (attribute.Type is AttributeType.Numeric)
            {
                dialog.ViewModel.AttributeType = dialog.ViewModel.NumericType;
            }
            else if (attribute.Type is AttributeType.Nominal nominal)
            {
                foreach (var label in nominal.NominalLabels)
                {
                    dialog.ViewModel.NominalType.NominalLabels.Add(label);
                }
                dialog.ViewModel.AttributeType = dialog.ViewModel.NominalType;
            }
            else if (attribute.Type is AttributeType.Binary binary)
            {
                dialog.ViewModel.BinaryType.TrueLabel = binary.TrueLabel;
                dialog.ViewModel.BinaryType.FalseLabel = binary.FalseLabel;
                dialog.ViewModel.AttributeType = dialog.ViewModel.BinaryType;
            }
            if (dialog.ShowDialog() == true)
            {
                return new RelationAttribute(dialog.ViewModel.Name, dialog.ViewModel.AttributeType,
                    new ObservableCollection<Value>());
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
