document.addEventListener('DOMContentLoaded', function () {
  var datosGastos = JSON.parse(document.getElementById("datosGastos").textContent);

  if (!datosGastos || datosGastos.length === 0) {
    document.getElementById("graficoBarras").innerHTML = "<p>No hay datos para mostrar.</p>";
    document.getElementById("graficoDonut").innerHTML = "<p>No hay datos para mostrar.</p>";
    return;
  }

  const categorias = datosGastos.map(gasto => gasto.Categoria);
  const totales = datosGastos.map(gasto => gasto.Total);

  // Paleta de colores personalizada
  const coloresPersonalizados = [
    "#1F77B4", "#FF7F0E", "#2CA02C", "#D62728", "#9467BD",
    "#8C564B", "#E377C2", "#7F7F7F", "#BCBD22", "#17BECF",
    "#AEC7E8", "#FFBB78", "#98DF8A", "#FF9896", "#C5B0D5",
    "#C49C94", "#F7B6D2", "#C7C7C7", "#DBDB8D", "#9EDAE5",
    "#5254A3", "#8CA252"
  ];

  setTimeout(() => {
    // Gráfico de Barras con colores distintos por categoría
    var options1 = {
      chart: {
        type: 'bar',
        height: 350,
        toolbar: {
          show: true,
          tools: {
            download: ['png', 'csv']
          }
        }
      },
      series: [{
        name: 'Total Gastado',
        data: totales
      }],
      plotOptions: {
        bar: {
          distributed: true, // Asegura que cada barra tenga un color único
          horizontal: false, // Asegura que el gráfico sea vertical
        }
      },
      colors: coloresPersonalizados, // Colores asignados a las barras
      xaxis: {
        categories: categorias
      },
      title: {
        text: 'Gastos por Categoría'
      }
    };

    var chart1 = new ApexCharts(document.querySelector("#graficoBarras"), options1);
    chart1.render();

    // Gráfico de Donut
    var options2 = {
      chart: {
        type: 'donut',
        height: 350,
        toolbar: {
          show: true,
          tools: {
            download: ['png', 'csv']
          }
        }
      },
      series: totales,
      labels: categorias,
      colors: coloresPersonalizados,
      title: {
        text: 'Distribución de Gastos por Categoría'
      }
    };

    var chart2 = new ApexCharts(document.querySelector("#graficoDonut"), options2);
    chart2.render();
  }, 500);
});
