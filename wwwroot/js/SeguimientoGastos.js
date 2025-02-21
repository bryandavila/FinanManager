document.addEventListener('DOMContentLoaded', function () {
  // Gráfico Comparación de Gastos entre Meses
  var comparisonChartOptions = {
    chart: {
      type: 'bar',
      height: 300,
    },
    series: [{
      name: 'Gastos',
      data: [450, 520, 610, 700, 670] // Datos de ejemplo
    }],
    xaxis: {
      categories: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo'],
    },
    colors: ['#00aaff'],
    title: {
      text: 'Comparación de Gastos entre Meses',
      align: 'center',
    }
  };
  var comparisonChart = new ApexCharts(document.querySelector("#comparisonChart"), comparisonChartOptions);
  comparisonChart.render();

  // Gráfico Planificado vs Real
  var plannedVsActualOptions = {
    chart: {
      type: 'line',
      height: 300,
    },
    series: [
      {
        name: 'Planificado',
        data: [400, 450, 500, 550, 600] // Datos de ejemplo
      },
      {
        name: 'Real',
        data: [380, 480, 520, 580, 610] // Datos de ejemplo
      }
    ],
    xaxis: {
      categories: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo'],
    },
    colors: ['#008ffb', '#00e396'],
    title: {
      text: 'Planificado vs Real',
      align: 'center',
    }
  };
  var plannedVsActualChart = new ApexCharts(document.querySelector("#plannedVsActualChart"), plannedVsActualOptions);
  plannedVsActualChart.render();
});
