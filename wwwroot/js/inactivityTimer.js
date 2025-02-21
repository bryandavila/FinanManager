// Tiempo de inactividad en milisegundos (2 minutos)
const inactivityTime = 2 * 60 * 1000; // 2 minutos

let inactivityTimer;

// Función para redirigir al login
function redirectToLogin() {
    window.location.href = '/Auth/SessionExpired';
}

// Reiniciar el temporizador de inactividad
function resetInactivityTimer() {
  clearTimeout(inactivityTimer);
  inactivityTimer = setTimeout(redirectToLogin, inactivityTime);
}

// Eventos que reinician el temporizador
document.addEventListener('mousemove', resetInactivityTimer);
document.addEventListener('keypress', resetInactivityTimer);
document.addEventListener('click', resetInactivityTimer);

// Iniciar el temporizador al cargar la página
resetInactivityTimer();
