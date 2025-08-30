document.addEventListener('DOMContentLoaded', function () {
  const compararBtn = document.getElementById('compararMesesBtn');
  let comparisonChart;

  compararBtn.addEventListener('click', function () {
    const mes1 = document.getElementById('mes1').value;
    const mes2 = document.getElementById('mes2').value;
    const anio = document.getElementById('anioComparacion').value;

    if (mes1 === mes2) {
      alert("Seleccione dos meses diferentes.");
      return;
    }

    const url = `/SeguimientoGastos/CompararMeses?meses=${mes1}&meses=${mes2}&anio=${anio}`;

    fetch(url)
      .then(response => {
        if (!response.ok) throw new Error("Error al obtener los datos");
        return response.json();
      })
      .then(data => {
        const tbody = document.querySelector('#tablaComparacion tbody');
        tbody.innerHTML = '';

        const categorias = [];
        const valores = [];

        for (const mes in data) {
          categorias.push(mes);
          valores.push(data[mes]);

          const fila = document.createElement('tr');
          fila.innerHTML = `
            <td>${mes}</td>
            <td>₡ ${data[mes].toLocaleString(undefined, { minimumFractionDigits: 2 })}</td>
          `;
          tbody.appendChild(fila);
        }

        // Renderizar gráfico solo con los dos meses seleccionados
        if (comparisonChart) {
          comparisonChart.updateOptions({
            series: [{ name: 'Gastos', data: valores }],
            xaxis: { categories: categorias }
          });
        } else {
          comparisonChart = new ApexCharts(document.querySelector("#comparisonChart"), {
            chart: { type: 'bar', height: 300 },
            series: [{ name: 'Gastos', data: valores }],
            xaxis: { categories: categorias },
            colors: ['#00aaff'],
            title: { text: 'Comparación de Gastos entre Meses', align: 'center' }
          });
          comparisonChart.render();
        }
      })
      .catch(error => {
        console.error("Error:", error);
        alert("No se pudieron obtener los datos para la comparación.");
      });
  });
});
