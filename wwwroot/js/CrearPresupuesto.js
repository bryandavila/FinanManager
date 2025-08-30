document.addEventListener('DOMContentLoaded', function () {
  const tipoPresupuesto = document.getElementById('tipoPresupuesto');
  const formularioPresupuesto = document.getElementById('formularioPresupuesto');

  tipoPresupuesto.addEventListener('change', function () {
    formularioPresupuesto.innerHTML = ''; // Limpia el contenido dinámico
    switch (this.value) {
      case 'Bienes':
        formularioPresupuesto.innerHTML = `
                    <div class="col-12">
                        <h5>Formulario de Bienes</h5>
                        <form id="formBienes">
                            <div class="mb-3">
                                <label for="tipoBien" class="form-label">Tipo</label>
                                <input type="text" class="form-control" id="tipoBien" name="tipoBien" placeholder="Escriba el tipo de bien">
                            </div>
                            <div class="mb-3">
                                <label for="descripcionBien" class="form-label">Descripción</label>
                                <input type="text" class="form-control" id="descripcionBien" name="descripcionBien" placeholder="Descripción del bien">
                            </div>
                            <div class="mb-3">
                                <label for="cantidad" class="form-label">Cantidad</label>
                                <input type="number" class="form-control" id="cantidad" name="cantidad" placeholder="Cantidad requerida">
                            </div>
                            <div class="mb-3">
                                <label for="montoUnitario" class="form-label">Monto Unitario</label>
                                <input type="number" class="form-control" id="montoUnitario" name="montoUnitario" placeholder="Monto por unidad">
                            </div>
                            <div class="mb-3">
                                <label for="total" class="form-label">Total</label>
                                <input type="number" class="form-control" id="total" name="total" placeholder="Monto total" readonly>
                            </div>
                            <div class="mb-3">
                                <label for="periodoEjecucion" class="form-label">Periodo de Ejecución</label>
                                <div id="periodoEjecucion" class="checkbox-container">
                                    <div class="checkbox-item"><label><input type="checkbox" name="enero" value="1"> Enero</label></div>
                                    <div class="checkbox-item"><label><input type="checkbox" name="febrero" value="1"> Febrero</label></div>
                                    <div class="checkbox-item"><label><input type="checkbox" name="marzo" value="1"> Marzo</label></div>
                                    <div class="checkbox-item"><label><input type="checkbox" name="abril" value="1"> Abril</label></div>
                                    <div class="checkbox-item"><label><input type="checkbox" name="mayo" value="1"> Mayo</label></div>
                                    <div class="checkbox-item"><label><input type="checkbox" name="junio" value="1"> Junio</label></div>
                                    <div class="checkbox-item"><label><input type="checkbox" name="julio" value="1"> Julio</label></div>
                                    <div class="checkbox-item"><label><input type="checkbox" name="agosto" value="1"> Agosto</label></div>
                                    <div class="checkbox-item"><label><input type="checkbox" name="septiembre" value="1"> Septiembre</label></div>
                                    <div class="checkbox-item"><label><input type="checkbox" name="octubre" value="1"> Octubre</label></div>
                                    <div class="checkbox-item"><label><input type="checkbox" name="noviembre" value="1"> Noviembre</label></div>
                                    <div class="checkbox-item"><label><input type="checkbox" name="diciembre" value="1"> Diciembre</label></div>
                                </div>
                            </div>
                            <button type="submit" class="btn btn-primary">Enviar</button>
                        </form>
                    </div>
                `;

        // Función para calcular el monto total
        const cantidad = document.getElementById('cantidad');
        const montoUnitario = document.getElementById('montoUnitario');
        const total = document.getElementById('total');

        function calcularMontoTotal() {
          const cantidadValor = parseFloat(cantidad.value);
          const montoUnitarioValor = parseFloat(montoUnitario.value);

          if (!isNaN(cantidadValor) && !isNaN(montoUnitarioValor)) {
            total.value = (cantidadValor * montoUnitarioValor).toFixed(2);
          } else {
            total.value = '';
          }
        }

        cantidad.addEventListener('input', calcularMontoTotal);
        montoUnitario.addEventListener('input', calcularMontoTotal);

        // Enviar formulario de Bienes
        document.getElementById('formBienes').addEventListener('submit', function (e) {
          e.preventDefault();
          const formData = new FormData(this);
          const data = {};
          formData.forEach((value, key) => {
            data[key] = value;
          });

          fetch('/CrearPresupuesto/CrearBien', {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
            },
            body: JSON.stringify(data),
          })
            .then(response => response.json())
            .then(data => {
              if (data.success) {
                Swal.fire({
                  icon: 'success',
                  title: '¡Éxito!',
                  text: data.message
                });
                this.reset();
              } else {
                Swal.fire({
                  icon: 'error',
                  title: 'Error',
                  text: data.message
                });
              }
            })
            .catch((error) => {
              console.error('Error:', error);
              Swal.fire({
                icon: 'error',
                title: 'Error de Conexión',
                text: 'No se pudo completar la solicitud.'
              });
            });
        });
        break;

      case 'Gastos':
        formularioPresupuesto.innerHTML = `
                    <div class="col-12">
                        <h5>Formulario de Gastos</h5>
                        <form id="formGastos">
                            <div class="mb-3">
                                <label for="cuentaTipoGasto" class="form-label">Cuenta y Tipo de Gasto</label>
                                <select class="form-select" id="cuentaTipoGasto" name="cuentaTipoGasto">
                                    <option value="441">441 Gastos del Personal</option>
                                    <option value="442">442 Gastos Servicios Externos</option>
                                    <option value="443">443 Gastos Movilidad y Comunicación</option>
                                    <option value="444">444 Gastos Infraestructura</option>
                                    <option value="445">445 Gastos Generales</option>
                                </select>
                            </div>
                            <div class="mb-3" id="subCuentaGastoContainer"></div>
                            <div class="mb-3">
                                <label for="justificacion" class="form-label">Justificación</label>
                                <textarea class="form-control" id="justificacion" name="justificacion" rows="3" placeholder="Escriba la justificación"></textarea>
                            </div>
                            <div class="mb-3">
                                <label for="total" class="form-label">Total</label>
                                <input type="number" class="form-control" id="total" name="total" placeholder="Total del gasto">
                            </div>
                            <div class="mb-3">
                                <label for="periodoEjecucion" class="form-label">Periodo de Ejecución</label>
                                <div id="periodoEjecucion" class="checkbox-container">
                                    <div class="checkbox-item"><label><input type="checkbox" name="enero" value="1"> Enero</label></div>
                                    <div class="checkbox-item"><label><input type="checkbox" name="febrero" value="1"> Febrero</label></div>
                                    <div class="checkbox-item"><label><input type="checkbox" name="marzo" value="1"> Marzo</label></div>
                                    <div class="checkbox-item"><label><input type="checkbox" name="abril" value="1"> Abril</label></div>
                                    <div class="checkbox-item"><label><input type="checkbox" name="mayo" value="1"> Mayo</label></div>
                                    <div class="checkbox-item"><label><input type="checkbox" name="junio" value="1"> Junio</label></div>
                                    <div class="checkbox-item"><label><input type="checkbox" name="julio" value="1"> Julio</label></div>
                                    <div class="checkbox-item"><label><input type="checkbox" name="agosto" value="1"> Agosto</label></div>
                                    <div class="checkbox-item"><label><input type="checkbox" name="septiembre" value="1"> Septiembre</label></div>
                                    <div class="checkbox-item"><label><input type="checkbox" name="octubre" value="1"> Octubre</label></div>
                                    <div class="checkbox-item"><label><input type="checkbox" name="noviembre" value="1"> Noviembre</label></div>
                                    <div class="checkbox-item"><label><input type="checkbox" name="diciembre" value="1"> Diciembre</label></div>
                                </div>
                            </div>
                            <button type="submit" class="btn btn-primary">Enviar</button>
                        </form>
                    </div>
                `;

        // Función para mostrar las subcuentas de tipo gasto
        function mostrarSubcuentas(cuenta) {
          const subCuentaGastoContainer = document.getElementById('subCuentaGastoContainer');
          subCuentaGastoContainer.innerHTML = '';  // Limpiar las subcuentas previas

          const opcionesSubcuentas = {
            '441': [
              '44101000 Sueldos y Bonificaciones',
              '44105000 Viáticos',
              '44106000 Décimo tercer sueldo',
              '44108000 Incentivos',
              '44112000 Cargas Sociales Patronales',
              '44113000 Refrigerios',
              '44114000 Vestimenta',
              '44115000 Capacitación',
              '44116000 Seguros para personal',
              '44199000 Otros gastos de personal'
            ],
            '442': [
              '44201000 Servicios de Computación',
              '44202000 Servicios de Seguridad',
              '44203000 Servicios de Información',
              '44204000 Servicios de Limpieza',
              '44205000 Asesoría Jurídica',
              '44206000 Auditoría Externa',
              '44207000 Consultoría Externa',
              '44208000 Servicios Médicos',
              '44210000 Servicios de Mensajería',
              '44212000 Servicios de Gestión de Riesgos',
              '44299000 Otros Servicios Contratados'
            ],
            '443': [
              '44301000 Pasajes y fletes',
              '44304000 Alquiler de Vehículos',
              '44307000 Teléfono y Fax',
              '44399000 Otros gastos de movilidad'
            ],
            '444': [
              '44401000 Seguros sobre bienes de uso',
              '44403000 Mantenimiento de inmuebles, mobiliario y equipo',
              '44404000 Agua y Energía Eléctrica',
              '44407000 Depreciación de inmuebles, mobiliario y equipo',
              '44499000 Otros gastos de infraestructura'
            ],
            '445': [
              '44503000 Otros seguros',
              '44505000 Amortización de Otros Cargos Diferidos',
              '44506000 Papelería, Útiles y otros materiales',
              '44507000 Gastos Legales',
              '44508000 Suscripciones y afiliaciones',
              '44509000 Promoción y Publicidad',
              '44512000 Amortización de Software',
              '44515000 Gastos por materiales y suministros',
              '44517000 Aporte al presupuesto de la SUGEF',
              '44599000 Gastos Generales Diversos'
            ]
          };

          const subcuentas = opcionesSubcuentas[cuenta] || [];
          const subCuentaSelect = document.createElement('select');
          subCuentaSelect.classList.add('form-select');
          subCuentaSelect.id = 'subCuentaGasto';
          subCuentaSelect.name = 'subCuentaGasto';
          subcuentas.forEach(subcuenta => {
            const option = document.createElement('option');
            option.value = subcuenta.split(' ')[0];
            option.textContent = subcuenta;
            subCuentaSelect.appendChild(option);
          });

          subCuentaGastoContainer.appendChild(subCuentaSelect);
        }

        // Llamamos a la función para mostrar las subcuentas según la cuenta seleccionada por defecto
        mostrarSubcuentas('441'); // Se puede cambiar por cualquier valor inicial de cuenta si es necesario

        // Evento para actualizar las subcuentas cuando se cambie la cuenta seleccionada
        document.getElementById('cuentaTipoGasto').addEventListener('change', function (event) {
          mostrarSubcuentas(event.target.value);
        });

        // Enviar formulario de Gastos
        document.getElementById('formGastos').addEventListener('submit', function (e) {
          e.preventDefault();

          // Recopilar los datos del formulario
          const formData = new FormData(this);
          const data = {};
          formData.forEach((value, key) => {
            data[key] = value;
          });

          // Agregar el código de la cuenta contable seleccionada
          const cuentaTipoGasto = parseInt(document.getElementById('cuentaTipoGasto').value); // Convertir a entero
          const subCuentaGasto = parseInt(document.getElementById('subCuentaGasto').value);   // Convertir a entero

          // Asignar los valores correctamente
          data.CuentaMadreId = cuentaTipoGasto; // Coincide con el nombre en el modelo
          data.CuentaHijaId = subCuentaGasto;   // Coincide con el nombre en el modelo

          // Mostrar los datos en la consola para depuración
          console.log("Datos enviados:", data);

          // Enviar la solicitud al backend
          fetch('/CrearPresupuesto/CrearGasto', {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
            },
            body: JSON.stringify(data),
          })
            .then(response => {
              if (!response.ok) {
                throw new Error('Error en la respuesta del servidor');
              }
              return response.json();
            })
            .then(data => {
              console.log("Respuesta del servidor:", data); // Ver la respuesta en la consola
              if (data.success) {
                // alert(data.message); <-- REEMPLAZADO
                Swal.fire({
                  icon: 'success',
                  title: '¡Éxito!',
                  text: data.message
                });
                this.reset();
              } else {
                Swal.fire({
                  icon: 'error',
                  title: 'Error',
                  text: data.message
                });
              }
            })
            .catch((error) => {
              console.error('Error:', error);
              Swal.fire({
                icon: 'error',
                title: 'Error de Conexión',
                text: 'No se pudo completar la solicitud.'
              });
            });
        });
        break;

      case 'Proyectos':
        formularioPresupuesto.innerHTML = `
                    <div class="col-12">
                        <h5>Formulario de Proyectos</h5>
                        <form id="formProyectos">
                            <div class="mb-3">
                                <label for="valorEstimado" class="form-label">Valor Estimado del Costo de Proyecto</label>
                                <input type="number" class="form-control" id="valorEstimado" name="valorEstimado" placeholder="Ingrese el costo estimado">
                            </div>
                            <div class="mb-3">
                                <label for="descripcion" class="form-label">Descripción del Proyecto</label>
                                <input type="text" class="form-control" id="descripcion" name="descripcion" placeholder="Escriba la descripción del proyecto">
                            </div>
                            <div class="mb-3">
                                <label for="viabilidadComercial" class="form-label">Viabilidad Comercial</label>
                                <input type="text" class="form-control" id="viabilidadComercial" name="viabilidadComercial" placeholder="Explique la viabilidad comercial">
                            </div>
                            <div class="mb-3">
                                <label for="viabilidadTecnica" class="form-label">Viabilidad Técnica</label>
                                <input type="text" class="form-control" id="viabilidadTecnica" name="viabilidadTecnica" placeholder="Explique la viabilidad técnica">
                            </div>
                            <div class="mb-3">
                                <label for="viabilidadLegal" class="form-label">Viabilidad Legal</label>
                                <input type="text" class="form-control" id="viabilidadLegal" name="viabilidadLegal" placeholder="Explique la viabilidad legal">
                            </div>
                            <div class="mb-3">
                                <label for="viabilidadGestion" class="form-label">Viabilidad de Gestión</label>
                                <input type="text" class="form-control" id="viabilidadGestion" name="viabilidadGestion" placeholder="Explique la viabilidad de gestión">
                            </div>
                            <div class="mb-3">
                                <label for="viabilidadImpactoAmbiental" class="form-label">Viabilidad de Impacto Ambiental</label>
                                <input type="text" class="form-control" id="viabilidadImpactoAmbiental" name="viabilidadImpactoAmbiental" placeholder="Explique la viabilidad de impacto ambiental">
                            </div>
                            <div class="mb-3">
                                <label for="viabilidadFinanciera" class="form-label">Viabilidad Financiera</label>
                                <input type="text" class="form-control" id="viabilidadFinanciera" name="viabilidadFinanciera" placeholder="Explique la viabilidad financiera">
                            </div>
                            <button type="submit" class="btn btn-primary">Enviar</button>
                        </form>
                    </div>
                `;

        // Enviar formulario de Proyectos
        document.getElementById('formProyectos').addEventListener('submit', function (e) {
          e.preventDefault();
          const formData = new FormData(this);
          const data = {};
          formData.forEach((value, key) => {
            data[key] = value;
          });

          console.log("Datos enviados:", JSON.stringify(data));

          fetch('/CrearPresupuesto/CrearProyecto', {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
            },
            body: JSON.stringify(data),
          })
            .then(response => response.json())
            .then(data => {
              console.log("Respuesta del servidor:", data);
              if (data.success) {
                Swal.fire({
                  icon: 'success',
                  title: '¡Éxito!',
                  text: data.message
                });
                this.reset();
              } else {
                Swal.fire({
                  icon: 'error',
                  title: 'Error',
                  text: data.message
                });
              }
            })
            .catch((error) => {
              console.error('Error:', error);
              Swal.fire({
                icon: 'error',
                title: 'Error de Conexión',
                text: 'No se pudo completar la solicitud.'
              });
            });
        });

        break;
    }
  });
});
