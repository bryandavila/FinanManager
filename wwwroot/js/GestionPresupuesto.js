'use strict';

(function () {
  let cardColor, headingColor, legendColor, labelColor, shadeColor, borderColor;

  cardColor = config.colors.cardColor;
  headingColor = config.colors.headingColor;
  legendColor = config.colors.bodyColor;
  labelColor = config.colors.textMuted;
  borderColor = config.colors.borderColor;

  // Presupuesto vs Gastos - Gráfico de barras
  const budgetVsExpenseChartEl = document.querySelector('#budgetVsExpenseChart'),
    budgetVsExpenseChartOptions = {
      series: [
        {
          name: 'Presupuesto',
          data: [30, 40, 35, 50, 55, 60, 70] // Datos de presupuesto mensual
        },
        {
          name: 'Gastos',
          data: [20, 30, 25, 45, 40, 55, 65] // Datos de gastos mensuales
        }
      ],
      chart: {
        height: 317,
        type: 'bar',
        toolbar: { show: false }
      },
      plotOptions: {
        bar: {
          horizontal: false,
          columnWidth: '30%',
          borderRadius: 8,
          startingShape: 'rounded',
          endingShape: 'rounded'
        }
      },
      colors: [config.colors.primary, config.colors.danger], // Colores para presupuesto y gastos
      dataLabels: {
        enabled: false
      },
      stroke: {
        curve: 'smooth',
        width: 6,
        lineCap: 'round',
        colors: [cardColor]
      },
      legend: {
        show: true,
        horizontalAlign: 'left',
        position: 'top',
        fontSize: '13px',
        fontFamily: 'Public Sans',
        fontWeight: 400,
        labels: {
          colors: legendColor,
          useSeriesColors: false
        },
        itemMargin: {
          horizontal: 10
        }
      },
      grid: {
        borderColor: borderColor,
        padding: {
          top: 0,
          bottom: -8,
          left: 20,
          right: 20
        }
      },
      fill: {
        opacity: [1, 1]
      },
      xaxis: {
        categories: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul'],
        labels: {
          style: {
            fontSize: '13px',
            fontFamily: 'Public Sans',
            colors: labelColor
          }
        },
        axisTicks: {
          show: false
        },
        axisBorder: {
          show: false
        }
      },
      yaxis: {
        labels: {
          style: {
            fontSize: '13px',
            fontFamily: 'Public Sans',
            colors: labelColor
          }
        }
      }
    };

  if (typeof budgetVsExpenseChartEl !== undefined && budgetVsExpenseChartEl !== null) {
    const budgetVsExpenseChart = new ApexCharts(budgetVsExpenseChartEl, budgetVsExpenseChartOptions);
    budgetVsExpenseChart.render();
  }

  // Progreso del Presupuesto - Gráfico de área
  const budgetProgressChartEl = document.querySelector('#budgetProgressChart'),
    budgetProgressChartOptions = {
      series: [
        {
          data: [15, 25, 20, 40, 45, 50, 60] // Progreso mensual del presupuesto
        }
      ],
      chart: {
        height: 232,
        type: 'area',
        toolbar: {
          show: false
        }
      },
      dataLabels: {
        enabled: false
      },
      stroke: {
        width: 3,
        curve: 'smooth'
      },
      colors: [config.colors.primary],
      fill: {
        type: 'gradient',
        gradient: {
          shade: 'dark',
          shadeIntensity: 0.6,
          opacityFrom: 0.5,
          opacityTo: 0.25,
          stops: [0, 95, 100]
        }
      },
      grid: {
        borderColor: borderColor,
        strokeDashArray: 8,
        padding: {
          top: -20,
          bottom: -8
        }
      },
      xaxis: {
        categories: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul'],
        labels: {
          style: {
            fontSize: '13px',
            fontFamily: 'Public Sans',
            colors: labelColor
          }
        }
      }
    };

  if (typeof budgetProgressChartEl !== undefined && budgetProgressChartEl !== null) {
    const budgetProgressChart = new ApexCharts(budgetProgressChartEl, budgetProgressChartOptions);
    budgetProgressChart.render();
  }

})();
