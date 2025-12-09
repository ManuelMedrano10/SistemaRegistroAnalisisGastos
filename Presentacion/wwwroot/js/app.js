let allGastos = [];
let allCategorias = [];
let allMetodos = [];

// Inicialización
document.addEventListener('DOMContentLoaded', () => {
    if (!localStorage.getItem('token')) {
        window.location.href = 'login.html';
    }
    loadInitialData();
});

function showSection(sectionId) {
    document.querySelectorAll('section').forEach(el => el.style.display = 'none');
    document.getElementById(`${sectionId}-section`).style.display = 'block';
    
    document.querySelectorAll('.menu-item').forEach(el => el.classList.remove('active'));
    event.target.classList.add('active');

    if(sectionId === 'gastos') loadGastos();
    if(sectionId === 'categorias') loadCategorias();
    if(sectionId === 'metodos') loadMetodos();
}

async function loadInitialData() {
    try {
        allCategorias = await Api.categorias.getAll();
        allMetodos = await Api.metodos.getAll();
        
        populateSelect('filter-cat', allCategorias, false);
        populateSelect('filter-met', allMetodos, false);
        populateSelect('gasto-cat', allCategorias, true);
        populateSelect('gasto-met', allMetodos, true);
        
        loadGastos();
    } catch (error) {
        console.error(error);
        showAlert(error.message, 'error');
    }
}


async function loadGastos() {
    try {
        allGastos = await Api.gastos.getAll();
        applyFilters(); // Renderiza la tabla con filtros aplicados
    } catch (error) {
        console.error(error);
        showAlert(error.message, 'error');
    }
}

function applyFilters() {
    const desc = document.getElementById('filter-desc').value.toLowerCase();
    const catName = document.getElementById('filter-cat').value; // Filtra por nombre porque el DTO Get trae nombre
    const metName = document.getElementById('filter-met').value;
    const dateStart = document.getElementById('filter-date-start').value;
    const dateEnd = document.getElementById('filter-date-end').value;

    const filtered = allGastos.filter(g => {
        const matchesDesc = g.descripcion.toLowerCase().includes(desc);
        const matchesCat = catName === "" || g.categoria === catName;
        const matchesMet = metName === "" || g.metodoPago === metName;
        
        let matchesDate = true;
        if (dateStart && dateEnd) {
            const gDate = new Date(g.fecha).toISOString().split('T')[0];
            matchesDate = gDate >= dateStart && gDate <= dateEnd;
        }

        return matchesDesc && matchesCat && matchesMet && matchesDate;
    });

    renderGastosTable(filtered);
}

function renderGastosTable(gastos) {
    const tbody = document.querySelector('#gastos-table tbody');
    tbody.innerHTML = '';
    let total = 0;

    gastos.forEach(g => {
        total += g.monto;
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td>${new Date(g.fecha).toLocaleDateString()}</td>
            <td>${g.descripcion}</td>
            <td>${g.categoria}</td>
            <td>${g.metodoPago}</td>
            <td>$${g.monto.toFixed(2)}</td>
            <td>
                <button onclick="editGasto(${g.id})" class="btn btn-sm btn-warning">Edit</button>
                <button onclick="deleteGasto(${g.id})" class="btn btn-sm btn-danger">Elim</button>
            </td>
        `;
        tbody.appendChild(tr);
    });
    document.getElementById('total-visible').innerText = `$${total.toFixed(2)}`;
}

const formGasto = document.getElementById('form-gasto');
formGasto.addEventListener('submit', async (e) => {
    e.preventDefault();
    const id = document.getElementById('gasto-id').value;
    const data = {
        descripcion: document.getElementById('gasto-desc').value,
        monto: parseFloat(document.getElementById('gasto-monto').value),
        fecha: document.getElementById('gasto-fecha').value,
        idCategoria: parseInt(document.getElementById('gasto-cat').value),
        idMetodoPago: parseInt(document.getElementById('gasto-met').value)
    };


    if(data.idCategoria === 0 || isNaN(data.idCategoria)) { alert("Seleccione una categoría válida"); return; }

    try {
        if (id) {
            data.id = parseInt(id);
            await Api.gastos.update(id, data);
        } else {
            await Api.gastos.create(data);
        }
        closeModal('modal-gasto');
        loadGastos();
        loadInitialData(); 
    } catch (error) {
        console.error(error);
        showAlert(error.message, 'error');
    }
});

function openGastoModal() {
    document.getElementById('form-gasto').reset();
    document.getElementById('gasto-id').value = "";
    document.getElementById('modal-gasto-title').innerText = "Nuevo Gasto";
    const now = new Date();
    now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
    document.getElementById('gasto-fecha').value = now.toISOString().slice(0,16);
    document.getElementById('modal-gasto').classList.add('show');
}

function editGasto(id) {
    const gasto = allGastos.find(g => g.id === id);
    if (!gasto) return;

    document.getElementById('gasto-id').value = gasto.id;
    document.getElementById('gasto-desc').value = gasto.descripcion;
    document.getElementById('gasto-monto').value = gasto.monto;
    document.getElementById('gasto-fecha').value = gasto.fecha.slice(0, 16); // Ajustar formato
    document.getElementById('modal-gasto-title').innerText = "Editar Gasto";

    if (gasto.idCategoria) {
        document.getElementById('gasto-cat').value = gasto.idCategoria;
    } else {
        const catObj = allCategorias.find(c => c.nombre === gasto.categoria);
        if (catObj) document.getElementById('gasto-cat').value = catObj.id;
    }

    if (gasto.idMetodoPago) {
        document.getElementById('gasto-met').value = gasto.idMetodoPago;
    } else {
        const metObj = allMetodos.find(m => m.nombre === gasto.metodoPago);
        if (metObj) document.getElementById('gasto-met').value = metObj.id;
    }

    document.getElementById('modal-gasto').classList.add('show');
}

async function deleteGasto(id) {
    if(confirm("¿Estás seguro de eliminar este gasto?")) {
        await Api.gastos.delete(id);
        loadGastos();
    }
}

function openImportModal() { document.getElementById('modal-import').classList.add('show'); }
document.getElementById('form-import').addEventListener('submit', async (e) => {
    e.preventDefault();
    const fileInput = document.getElementById('inputImportar');
    const archivo = fileInput.files[0];

    if (!archivo) {
        showAlert("Selecciona un archivo primero.", "error");
        return;
    }

    if (!archivo.name.endsWith('.xlsx')) {
        showAlert("El archivo debe ser un Excel (.xlsx)", "error");
        return;
    }

    const formData = new FormData();
    formData.append('archivo', fileInput.files[0]);

    try {
        const msg = await Api.gastos.import(formData);
        alert(msg);
        closeModal('modal-import');
        loadGastos();
    } catch (error) {
        console.error(error);
        showAlert(error.message, 'error');
    }
});


async function loadCategorias() {
    allCategorias = await Api.categorias.getAll();
    const container = document.getElementById('categorias-list');
    container.innerHTML = '';

    const currentMonth = new Date().getMonth();
    const currentYear = new Date().getFullYear();
    
    allCategorias.forEach(cat => {
        const gastado = allGastos
            .filter(g => {
                const d = new Date(g.fecha);
                return d.getMonth() === currentMonth && d.getFullYear() === currentYear && g.categoria === cat.nombre;
            })
            .reduce((sum, g) => sum + g.monto, 0);

        const porcentaje = cat.presupuesto > 0 ? (gastado / cat.presupuesto) * 100 : 0;
        
        let colorClass = "bg-success";
        let alertMsg = "";
        
        if (porcentaje >= 100) { colorClass = "#EF4444"; alertMsg = "¡Presupuesto Excedido!"; }
        else if (porcentaje >= 80) { colorClass = "#F59E0B"; alertMsg = "Cuidado: >80%"; }
        else if (porcentaje >= 50) { colorClass = "#3B82F6"; alertMsg = "Atención: >50%"; }
        else { colorClass = "#10B981"; }

        const div = document.createElement('div');
        div.className = 'card';
        div.innerHTML = `
            <div class="d-flex justify-between">
                <h3>${cat.nombre} ${cat.isActivo ? '' : '(Inactiva)'}</h3>
                <div>
                    <button onclick="editCategoria(${cat.id})" class="btn btn-sm btn-warning">✏️</button>
                    <button onclick="deleteCategoria(${cat.id})" class="btn btn-sm btn-danger">🗑️</button>
                </div>
            </div>
            <p>Presupuesto: $${cat.presupuesto}</p>
            <p>Gastado (Mes Actual): $${gastado.toFixed(2)}</p>
            <div style="font-weight: bold; color: ${colorClass}; height: 20px;">${alertMsg}</div>
            
            <div class="progress-bar">
                <div class="progress-fill" style="width: ${Math.min(porcentaje, 100)}%; background-color: ${colorClass};"></div>
            </div>
            <small>${porcentaje.toFixed(1)}%</small>
        `;
        container.appendChild(div);
    });
}

const formCategoria = document.getElementById('form-categoria');
formCategoria.addEventListener('submit', async (e) => {
    e.preventDefault();
    const id = document.getElementById('cat-id').value;
    const data = {
        id: id ? parseInt(id) : 0,
        nombre: document.getElementById('cat-nombre').value,
        presupuesto: parseFloat(document.getElementById('cat-presupuesto').value),
        isActivo: document.getElementById('cat-activo').value === "true"
    };

    try {
        if(id) await Api.categorias.update(id, data);
        else await Api.categorias.create(data);
        
        closeModal('modal-categoria');
        loadCategorias();
        loadInitialData();
    } catch (error) {
        console.error(error);
        showAlert(error.message, 'error');
    }
});

function openCategoriaModal() {
    document.getElementById('form-categoria').reset();
    document.getElementById('cat-id').value = "";
    document.getElementById('modal-categoria').classList.add('show');
}
function editCategoria(id) {
    const cat = allCategorias.find(c => c.id === id);
    document.getElementById('cat-id').value = cat.id;
    document.getElementById('cat-nombre').value = cat.nombre;
    document.getElementById('cat-presupuesto').value = cat.presupuesto;
    document.getElementById('cat-activo').value = cat.isActivo;
    document.getElementById('modal-categoria').classList.add('show');
}
async function deleteCategoria(id) {
    if(confirm("Eliminar categoría?")) {
        try { await Api.categorias.delete(id); loadCategorias(); } 
        catch (error) {
            console.error(error);
            showAlert(error.message, 'error');
        }
    }
}


async function loadMetodos() {
    allMetodos = await Api.metodos.getAll();
    const tbody = document.querySelector('#metodos-table tbody');
    tbody.innerHTML = '';
    allMetodos.forEach(m => {
        tbody.innerHTML += `
                <td>${m.nombre}</td>
                <td>
                    <button onclick="editMetodo(${m.id})" class="btn btn-sm btn-warning">Edit</button>
                    <button onclick="deleteMetodo(${m.id})" class="btn btn-sm btn-danger">Elim</button>
                </td>
            </tr>`;
    });
}

function openMetodoModal() { document.getElementById('form-metodo').reset(); document.getElementById('met-id').value=""; document.getElementById('modal-metodo').classList.add('show'); }
function editMetodo(id) { 
    const m = allMetodos.find(x => x.id === id); 
    document.getElementById('met-id').value = m.id; 
    document.getElementById('met-nombre').value = m.nombre; 
    document.getElementById('modal-metodo').classList.add('show'); 
}
async function deleteMetodo(id) { if(confirm("Eliminar?")) { await Api.metodos.delete(id); loadMetodos(); } }
document.getElementById('form-metodo').addEventListener('submit', async(e) => {
    e.preventDefault();
    const id = document.getElementById('met-id').value;
    const nombre = document.getElementById('met-nombre').value;
    try {
        if(id) await Api.metodos.update(id, { id: parseInt(id), nombre });
        else await Api.metodos.create({ nombre });
        closeModal('modal-metodo'); loadMetodos(); loadInitialData();
    } catch (error) {
        console.error(error);
        showAlert(error.message, 'error');
    }
});

async function downloadReport(format) {
    const year = document.getElementById('report-year').value;
    const month = document.getElementById('report-month').value;
    try {
        const blob = await Api.reportes.descargar(year, month, format);
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `Reporte_${year}_${month}.${format}`;
        document.body.appendChild(a);
        a.click();
        a.remove();
        window.URL.revokeObjectURL(url);
    } catch (error) {
        console.error(error);
        showAlert(error.message, 'error');
    }
}

async function updateName() {
    const email = localStorage.getItem('userEmail');
    const name = document.getElementById('profile-name').value;
    try { await Api.auth.updateProfile(email, name); alert("Nombre actualizado"); }
    catch (eerror) {
        console.error(error);
        showAlert(error.message, 'error');
    }
}
async function updatePass() {
    const email = localStorage.getItem('userEmail');
    const data = {
        claveActual: document.getElementById('old-pass').value,
        confirmarClave: document.getElementById('confirm-pass').value,
        nuevaClave: document.getElementById('new-pass').value
    };
    try { await Api.auth.updatePassword(email, data); alert("Contraseña actualizada"); }
    catch (error) {
        console.error(error);
        showAlert(error.message, 'error');
    }
}

function closeModal(id) { document.getElementById(id).classList.remove('show'); }
function logout() { localStorage.clear(); window.location.href = 'login.html'; }
    function populateSelect(id, items, useId) {
        const sel = document.getElementById(id);
        const prev = sel.value;
        sel.innerHTML = useId ? '' : '<option value="">Todos</option>';
        items.forEach(i => {
            const opt = document.createElement('option');
            opt.value = useId ? i.id : i.nombre;
            opt.text = i.nombre;
            sel.appendChild(opt);
        });
        sel.value = prev;
    }
