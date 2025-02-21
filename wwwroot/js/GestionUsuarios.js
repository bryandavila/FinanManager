// Función para mostrar un diálogo de confirmación reutilizable
function showConfirmationDialog(options, callback) {
  Swal.fire({
    title: options.title || "¿Estás seguro?",
    text: options.text || "Esta acción no se puede deshacer.",
    icon: "warning",
    showCancelButton: true,
    confirmButtonColor: "#3085d6",
    cancelButtonColor: "#d33",
    confirmButtonText: options.confirmButtonText || "Sí, continuar",
    cancelButtonText: options.cancelButtonText || "Cancelar"
  }).then((result) => {
    if (result.isConfirmed && typeof callback === "function") {
      callback();
    }
  });
}

// Función para mostrar alertas de éxito o error
function showAlert(options) {
  Swal.fire({
    title: options.title || "Mensaje",
    text: options.text || "",
    icon: options.icon || "info",
    confirmButtonText: options.confirmButtonText || "OK"
  });
}

// Función genérica para hacer peticiones AJAX
function sendRequest(url, data, successCallback) {
  $.ajax({
    url: url,
    type: 'POST',
    data: data,
    success: function (response) {
      if (response.success) {
        showAlert({ title: "¡Éxito!", text: response.message, icon: "success" });
        if (typeof successCallback === "function") successCallback(response);
      } else {
        showAlert({ title: "Error", text: response.message || "No se pudo completar la acción.", icon: "error" });
      }
    },
    error: function () {
      showAlert({ title: "Error", text: "Ocurrió un problema en el servidor.", icon: "error" });
    }
  });
}

// Función para activar/desactivar usuario
function toggleUserStatus(userId, currentStatus) {
  showConfirmationDialog(
    {
      title: "¿Cambiar estado del usuario?",
      text: "Esto afectará su acceso al sistema.",
      confirmButtonText: "Sí, cambiar"
    },
    function () {
      sendRequest('/Administrador/ActivateOrDeactivateUser', { id: userId, bit: currentStatus }, function () {
        // Calcular el nuevo estado
        let newStatus = currentStatus === 1 ? 0 : 1;

        // Actualizar el texto del estado en la tabla
        $(`#status-${userId}`).text(newStatus === 1 ? "Activo" : "Inactivo");

        // Actualizar el botón
        let btn = $(`#toggle-btn-${userId}`);
        btn.text(newStatus === 1 ? "Desactivar" : "Activar");
        btn.removeClass(newStatus === 1 ? "btn-success" : "btn-secondary")
          .addClass(newStatus === 1 ? "btn-secondary" : "btn-success");
        btn.attr("onclick", `toggleUserStatus(${userId}, ${newStatus})`);
      });
    }
  );
}



// Función para eliminar un usuario y actualizar la vista
function deleteUser(userId) {
  showConfirmationDialog({
    title: "¿Eliminar usuario?",
    text: "Esta acción no se puede deshacer.",
    confirmButtonText: "Sí, eliminar"
  }, function () {
    sendRequest('/Administrador/Delete', { id: userId }, function () {
      // Eliminar la fila de la tabla al completar la eliminación
      $(`#row-${userId}`).remove();
    });
  });
}

// Función para manejar la confirmación antes de guardar los cambios de un usuario
// Función para manejar la confirmación antes de guardar los cambios
function handleSaveConfirmation() {
  showConfirmationDialog({
    title: "¿Estás seguro de guardar los cambios?",
    text: "Asegúrate de que los datos sean correctos antes de continuar.",
    confirmButtonText: "Sí, guardar cambios",
  }, function () {
    // Si el usuario confirma, se envía el formulario
    $("form").submit(); // Envía el formulario para guardar los cambios
  });
}

// Agrega un event listener para el botón "Guardar Cambios"
$("#saveButton").click(function () {
  handleSaveConfirmation(); // Llama a la función con nombre
});





// Función para restablecer contraseña de un usuario
function resetUserPassword(userId) {
  showConfirmationDialog(
    {
      title: "¿Restablecer contraseña?",
      text: "Se generará una nueva contraseña para el usuario.",
      confirmButtonText: "Sí, restablecer"
    },
    function () {
      sendRequest('/Administrador/ResetPassword', { id: userId }, function () {
        showAlert({ title: "¡Éxito!", text: "La contraseña ha sido restablecida.", icon: "success" });
      });
    }
  );
}


