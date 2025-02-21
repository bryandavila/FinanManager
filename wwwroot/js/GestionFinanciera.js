document.addEventListener("DOMContentLoaded", function () {
  // Asegurarse de que el contenedor tiene el tamaño correcto
  var chartContainer = document.querySelector("#comparativoAnualChart");
  if (chartContainer) {
    chartContainer.style.height = '350px';
  }

  // Reemplaza los datos con los que deseas mostrar en tus gráficos
  var optionsAnual = {
    series: [{
      name: 'Presupuesto 2024',
      data: [5000, 7000, 9000, 12000, 15000, 18000] // Ejemplo de datos mensuales 2024
    }, {
      name: 'Presupuesto 2023',
      data: [4500, 6500, 8500, 11000, 14000, 17000] // Ejemplo de datos mensuales 2023
    }],
    chart: {
      height: 350,
      type: 'line',
    },
    title: {
      text: 'Comparativo Anual de Presupuestos',
      align: 'left'
    },
    xaxis: {
      categories: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun'], // Meses del año
    },
    yaxis: {
      title: {
        text: 'Monto ($)',
      },
    },
    tooltip: {
      y: {
        formatter: function (val) {
          return "$" + val.toFixed(2);
        }
      }
    },
  };

  // Inicializar y renderizar el gráfico
  var chartAnual = new ApexCharts(document.querySelector("#comparativoAnualChart"), optionsAnual);
  chartAnual.render();
});
