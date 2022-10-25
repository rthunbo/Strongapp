using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Strongapp.Models;
using Strongapp.UI.Modals;
using Strongapp.UI.Services;

namespace Strongapp.UI.Pages
{
    public partial class Measure
    {
        [CascadingParameter]
        public IModalService Modal { get; set; }

        [Inject]
        public IMeasurementService MeasurementService { get; set; }

        public List<StrongMeasurement> Measurements { get; set; }

        public StrongMeasurement? MostRecentWeightMeasurement => Measurements.Where(x => x.Name == "Weight").OrderBy(x => x.Date).LastOrDefault();

        protected override async Task OnInitializedAsync()
        {
            Measurements = await MeasurementService.GetMeasurements();
        }

        public async Task ShowMeasurementHistoryModal(string measurementName)
        {
            var measurements = Measurements.Where(x => x.Name == measurementName).OrderByDescending(x => x.Date).ToList();
            var modalParameters = new ModalParameters();
            modalParameters.Add("Measurements", measurements);
            Modal.Show<MeasurementHistoryModal>(measurementName, modalParameters);
        }

        public async Task ShowAddMeasurementModal(string measurementName, string measurementUnit)
        {
            var modalParameters = new ModalParameters();
            modalParameters.Add("MeasurementName", measurementName);
            modalParameters.Add("MeasurementUnit", measurementUnit);
            var formModal = Modal.Show<AddMeasurementModal>("Add Measurement", modalParameters);
            var result = await formModal.Result;
            if (!result.Cancelled)
            {
                var measurement = (StrongMeasurement)result.Data;
                await MeasurementService.CreateMeasurement(measurement);
                Measurements = await MeasurementService.GetMeasurements();
            }
        }
    }
}
