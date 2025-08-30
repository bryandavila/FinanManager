document.addEventListener("DOMContentLoaded", function () {
  // Elementos del formulario
  const cantidadInput = document.getElementById('cantidadProyeccion');
  const montoUnitarioInput = document.getElementById('montoUnitarioProyeccion');
  const montoTotalInput = document.getElementById('montoTotalProyeccion');

  // Función para calcular el monto total automáticamente
  function calcularMontoTotal() {
    const cantidad = parseFloat(cantidadInput.value) || 0; // Valor por defecto 0 si no es numérico
    const montoUnitario = parseFloat(montoUnitarioInput.value) || 0; // Valor por defecto 0 si no es numérico
    const montoTotal = cantidad * montoUnitario;
    montoTotalInput.value = Math.floor(montoTotal); // Mostrar sin decimales
  }

  // Event listeners para recalcular el monto total al cambiar la cantidad o monto unitario
  cantidadInput.addEventListener('input', calcularMontoTotal);
  montoUnitarioInput.addEventListener('input', calcularMontoTotal);

  // Comprobación del formulario antes de enviarlo
  const form = document.querySelector('form');
  form.addEventListener('submit', function (event) {
    event.preventDefault(); // Prevenir el envío del formulario para validarlo

    // Validación de campos
    const tipoPresupuesto = document.getElementById('tipoPresupuesto').value;
    const descripcionBien = document.getElementById('descripcionBien').value.trim();
    const periodoEjecucion = document.querySelectorAll('#periodoEjecucion input[type="checkbox"]:checked');

    if (!descripcionBien) {
      alert('La descripción es obligatoria.');
      return;
    }

    if (periodoEjecucion.length === 0) {
      alert('Debes seleccionar al menos un mes para el periodo de ejecución.');
      return;
    }

    // Si todo es válido, podemos enviar el formulario o hacer lo que necesitemos
    alert(`Formulario de tipo de presupuesto "${tipoPresupuesto}" listo para guardar.`);
    // Aquí podrías usar AJAX o redirigir a otro lugar
  });
});
