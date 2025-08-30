document.addEventListener('DOMContentLoaded', function () {
  // Exportar a PDF
  document.getElementById('exportPdfBtn').addEventListener('click', function () {
    const { jsPDF } = window.jspdf;
    const doc = new jsPDF();

    doc.autoTable({
      html: '#gastosMensuales',
      styles: { fontSize: 10 }
    });

    doc.save('Detalle_Gastos_Mensuales.pdf');
  });

  // Exportar a Excel
  document.getElementById('exportExcelBtn').addEventListener('click', function () {
    const table = document.getElementById('gastosMensuales');
    const rows = table.querySelectorAll('tr');
    let csvContent = "data:text/csv;charset=utf-8,";

    rows.forEach(row => {
      const cols = row.querySelectorAll('td, th');
      const rowData = Array.from(cols).map(col => col.innerText).join(",");
      csvContent += rowData + "\r\n";
    });

    const encodedUri = encodeURI(csvContent);
    const link = document.createElement('a');
    link.setAttribute('href', encodedUri);
    link.setAttribute('download', 'Detalle_Gastos_Mensuales.csv');
    document.body.appendChild(link);
    link.click();
  });
});
