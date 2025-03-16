document.addEventListener("DOMContentLoaded", function () {
  // Verificar que el contenedor del gráfico exista
  const chartContainer = document.querySelector("#comparativoAnualChart");
  if (!chartContainer) {
    console.error("El contenedor #comparativoAnualChart no existe en el DOM.");
    return; // Detener la ejecución si el contenedor no existe
  }

  // Asegurarse de que el contenedor tenga el tamaño correcto
  chartContainer.style.height = '350px';

  // Configuración del gráfico
  const optionsAnual = {
    series: [{
      name: 'Presupuesto 2024',
      data: [5000, 7000, 9000, 12000, 15000, 18000] // Datos mensuales para 2024
    }, {
      name: 'Presupuesto 2023',
      data: [4500, 6500, 8500, 11000, 14000, 17000] // Datos mensuales para 2023
    }],
    chart: {
      height: 350,
      type: 'line',
      zoom: {
        enabled: false // Deshabilitar zoom para este gráfico
      }
    },
    title: {
      text: 'Comparativo Anual de Presupuestos',
      align: 'left',
      style: {
        fontSize: '16px',
        fontWeight: 'bold'
      }
    },
    xaxis: {
      categories: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun'], // Meses del año
      title: {
        text: 'Meses',
        style: {
          fontSize: '14px',
          fontWeight: 'bold'
        }
      }
    },
    yaxis: {
      title: {
        text: 'Monto ($)',
        style: {
          fontSize: '14px',
          fontWeight: 'bold'
        }
      },
      labels: {
        formatter: function (val) {
          return "$" + val.toFixed(2); // Formato de los valores en el eje Y
        }
      }
    },
    tooltip: {
      y: {
        formatter: function (val) {
          return "$" + val.toFixed(2); // Formato de los valores en el tooltip
        }
      }
    },
    stroke: {
      width: 2, // Grosor de las líneas
      curve: 'smooth' // Líneas suavizadas
    },
    markers: {
      size: 5, // Tamaño de los marcadores
      colors: ['#008FFB', '#00E396'], // Colores de los marcadores
      strokeWidth: 0 // Sin borde en los marcadores
    },
    legend: {
      position: 'top', // Posición de la leyenda
      horizontalAlign: 'right', // Alineación horizontal de la leyenda
      fontSize: '14px'
    }
  };

  try {
    // Inicializar y renderizar el gráfico
    const chartAnual = new ApexCharts(chartContainer, optionsAnual);
    chartAnual.render();
  } catch (error) {
    console.error("Error al inicializar el gráfico:", error);
  }
});
