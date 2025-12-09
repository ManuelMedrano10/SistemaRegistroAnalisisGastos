const BASE_URL = "https://localhost:7202/api";

const Api = {
    getHeaders: () => {
        const token = localStorage.getItem("token");
        return {
            "Content-Type": "application/json",
            "Authorization": token ? `Bearer ${token}` : ""
        };
    },

    getHeadersMultipart: () => {
        const token = localStorage.getItem("token");
        return {
            "Authorization": token ? `Bearer ${token}` : ""
        };
    },

    async request(endpoint, method = "GET", body = null) {
        const options = {
            method,
            headers: Api.getHeaders()
        };
        if (body) options.body = JSON.stringify(body);

        try {
            const response = await fetch(`${BASE_URL}${endpoint}`, options);
            
            if (response.status === 401) {
                alert("Sesión expirada. Por favor inicie sesión nuevamente.");
                window.location.href = "login.html";
                return;
            }

            if (!response.ok) {
                const errorData = await response.json().catch(() => ({ message: "Error desconocido" }));
                throw new Error(errorData.message || `Error ${response.status}`);
            }

            if (response.status === 204) return null;

            return await response.json();
        } catch (error) {
            console.error("API Error:", error);
            throw error;
        }
    },

  
    auth: {
        login: (data) => Api.request("/Auth/login", "POST", data),
        register: (data) => Api.request("/Auth/register", "POST", data),
        updateProfile: (email, name) => Api.request(`/Auth/actualizarNombre/${email}?nombre=${name}`, "PUT"),
        updatePassword: (email, data) => Api.request(`/Auth/actualizarClave/${email}?claveActual=${data.claveActual}&confirmarClave=${data.confirmarClave}&nuevaClave=${data.nuevaClave}`, "PUT")
    },
    gastos: {
        getAll: () => Api.request("/Gasto"),
        create: (data) => Api.request("/Gasto", "POST", data),
        update: (id, data) => Api.request(`/Gasto/${id}`, "PUT", data),
        delete: (id) => Api.request(`/Gasto/${id}`, "DELETE"),
        import: async (formData) => {
            const response = await fetch(`${BASE_URL}/Gasto/importar`, {
                method: "POST",
                headers: Api.getHeadersMultipart(),
                body: formData
            });
            if (!response.ok) throw new Error("Error al importar archivo");
            return await response.text();
        }
    },
    categorias: {
        getAll: () => Api.request("/Categoria"),
        create: (data) => Api.request("/Categoria", "POST", data),
        update: (id, data) => Api.request(`/Categoria/${id}`, "PUT", data),
        delete: (id) => Api.request(`/Categoria/${id}`, "DELETE")
    },
    metodos: {
        getAll: () => Api.request("/MetodoPago"),
        create: (data) => Api.request("/MetodoPago", "POST", data),
        update: (id, data) => Api.request(`/MetodoPago/${id}`, "PUT", data),
        delete: (id) => Api.request(`/MetodoPago/${id}`, "DELETE")
    },
    reportes: {
        descargar: async (anio, mes, formato) => {
            const token = localStorage.getItem("token");
            const response = await fetch(`${BASE_URL}/ExportarReporte/descargar/${formato}?año=${anio}&mes=${mes}`, {
                headers: { "Authorization": `Bearer ${token}` }
            });
            if (!response.ok) throw new Error("Error descargando reporte");
            return await response.blob();
        }
    }
};

function showAlert(message, type = 'error') {
    const container = document.getElementById('alert-container');
    if (!container) return;

    container.className = 'alert-box';
    if (type === 'error') {
        container.classList.add('alert-error');
    } else {
        container.classList.add('alert-success');
    }

    container.textContent = message;
    container.style.display = 'block';

    setTimeout(() => {
        container.style.display = 'none';
    }, 5000);
};