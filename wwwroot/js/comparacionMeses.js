document.addEventListener('DOMContentLoaded', function () {
  const compararMesesBtn = document.getElementById('compararMesesBtn');
  const mes1Select = document.getElementById('mes1');
  const mes2Select = document.getElementById('mes2');
  const tablaComparacion = document.querySelector("#tablaComparacion tbody");

  compararMesesBtn.addEventListener('click', function () {
    const mes1 = mes1Select.value;
    const mes2 = mes2Select.value;

    if (!mes1 || !mes2 || mes1 === mes2) {
      alert('Por favor, selecciona dos meses distintos para comparar.');
      return;
    }

    fetch(`/SeguimientoGastos/CompararMeses?meses=${mes1}&meses=${mes2}`)
      .then(response => response.json())
      .then(data => {
        console.log(data); // Verifica los datos devueltos
        actualizarTablaComparacion(data);
      })
      .catch(error => console.error('Error:', error));
  });

  function actualizarTablaComparacion(data) {
    // Limpiar la tabla antes de agregar nuevos datos
    tablaComparacion.innerHTML = "";

    // Llenar la tabla con los datos devueltos
    for (const [mes, total] of Object.entries(data)) {
      const fila = document.createElement("tr");
      const celdaMes = document.createElement("td");
      const celdaTotal = document.createElement("td");

      celdaMes.textContent = mes;
      celdaTotal.textContent = total.toLocaleString("es-ES", { style: "currency", currency: "USD" });

      fila.appendChild(celdaMes);
      fila.appendChild(celdaTotal);
      tablaComparacion.appendChild(fila);
    }
  }
});
