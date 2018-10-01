﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AcManager.Tools.GameProperties;
using AcManager.Tools.Helpers;
using AcManager.Tools.Profile;
using AcTools.Processes;

namespace AcManager.Pages.Drive {
    public partial class QuickDrive_Hotlap : IQuickDriveModeControl {
        public class ViewModel : QuickDriveSingleModeViewModel {
            private bool _ghostCar;

            public bool GhostCar {
                get => _ghostCar;
                set {
                    if (value == _ghostCar) return;
                    _ghostCar = value;
                    OnPropertyChanged();
                    SaveLater();
                }
            }

            private double _ghostCarAdvantage;

            public double GhostCarAdvantage {
                get => _ghostCarAdvantage;
                set {
                    if (Equals(value, _ghostCarAdvantage)) return;
                    _ghostCarAdvantage = value;
                    OnPropertyChanged();
                    SaveLater();
                }
            }

            #region Saveable
            protected new class SaveableData : QuickDriveSingleModeViewModel.SaveableData {
                public bool GhostCar = true;
                public double GhostCarAdvantage;
            }

            protected override ISaveHelper CreateSaveable(string key) {
                return new SaveHelper<SaveableData>(key, () => Save(new SaveableData()), Load);
            }

            protected SaveableData Save(SaveableData data) {
                base.Save(data);
                data.GhostCar = GhostCar;
                data.GhostCarAdvantage = GhostCarAdvantage;
                return data;
            }

            protected void Load(SaveableData data) {
                base.Load(data);
                GhostCar = data.GhostCar;
                GhostCarAdvantage = data.GhostCarAdvantage;
            }

            public ViewModel(bool initialize = true) {
                Initialize("__QuickDrive_Hotlap", initialize);
            }
            #endregion

            public override async Task Drive(Game.BasicProperties basicProperties, Game.AssistsProperties assistsProperties,
                    Game.ConditionProperties conditionProperties, Game.TrackProperties trackProperties,
                    string serializedQuickDrivePreset, IList<object> additionalProperties) {
                basicProperties.Ballast = PlayerBallast;
                basicProperties.Restrictor = PlayerRestrictor;
                await StartAsync(new Game.StartProperties {
                    BasicProperties = basicProperties,
                    AssistsProperties = assistsProperties,
                    ConditionProperties = conditionProperties,
                    TrackProperties = trackProperties,
                    ModeProperties = new Game.HotlapProperties {
                        Penalties = Penalties,
                        GhostCar = GhostCar,
                        GhostCarAdvantage = GhostCarAdvantage,
                        RecordGhostCar = SettingsHolder.Drive.AlwaysRecordGhost ? true : (bool?)null
                    },
                    AdditionalPropertieses = additionalProperties.Concat(new object[] {
                        new QuickDrivePresetProperty(serializedQuickDrivePreset),
                        new CarCustomDataHelper()
                    }).ToList()
                });
            }
        }

        public QuickDrive_Hotlap() {
            InitializeComponent();
        }

        public QuickDriveModeViewModel Model {
            get => (QuickDriveModeViewModel)DataContext;
            set => DataContext = value;
        }
    }
}
