import React, { useState, useEffect } from 'react';
import { Search, Edit2, Shield, UserX, CheckCircle, XCircle, AlertCircle, X, CheckCircle2 } from 'lucide-react';

type AlertType = 'error' | 'success' | null;

interface UserAdminDto {
  id: number;
  email: string;
  role: string;
  firstName: string;
  lastName: string;
  phone?: string;
  isActive: boolean;
}

export default function AdminUsers() {
  const [users, setUsers] = useState<UserAdminDto[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [loading, setLoading] = useState(false);
  const [editingUser, setEditingUser] = useState<UserAdminDto | null>(null);
  const [userToDelete, setUserToDelete] = useState<number | null>(null);
  
  const [alertInfo, setAlertInfo] = useState<{ type: AlertType; message: string }>({ type: null, message: '' });

  useEffect(() => {
    if (alertInfo.type) {
      const timer = setTimeout(() => setAlertInfo({ type: null, message: '' }), 5000);
      return () => clearTimeout(timer);
    }
  }, [alertInfo]);

  const [isCreating, setIsCreating] = useState(false);
  const [newUser, setNewUser] = useState({
    firstName: '',
    lastName: '',
    email: '',
    phone: '',
    password: '',
    roleName: 'Cliente'
  });

  const fetchUsers = async () => {
    setLoading(true);
    try {
      const token = localStorage.getItem('token');
      const url = searchTerm 
        ? `http://localhost:5219/api/admin/users?search=${encodeURIComponent(searchTerm)}`
        : 'http://localhost:5219/api/admin/users';
        
      const res = await fetch(url, {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      if (res.ok) {
        setUsers(await res.json());
      }
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUsers();
  }, []);

  const handleUpdate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editingUser) return;
    
    try {
      const token = localStorage.getItem('token');
      const res = await fetch(`http://localhost:5219/api/admin/users/${editingUser.id}`, {
        method: 'PUT',
        headers: { 
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(editingUser)
      });
      
      if (res.ok) {
        setEditingUser(null);
        fetchUsers();
      } else {
        let errorMessage = 'Error al actualizar el usuario';
        try {
            const text = await res.text();
            if (text) {
                const errorData = JSON.parse(text);
                errorMessage = errorData.message || errorMessage;
            }
        } catch (parseError) {
            console.error("Error parsing response", parseError);
        }
        setAlertInfo({ type: 'error', message: errorMessage });
      }
    } catch (error: any) {
      console.error(error);
      setAlertInfo({ type: 'error', message: 'Error de conexión o de red: ' + error.message });
    }
  };

  const executeDelete = async (id: number) => {
    setUserToDelete(null);
    try {
      const token = localStorage.getItem('token');
      const res = await fetch(`http://localhost:5219/api/admin/users/${id}`, {
        method: 'DELETE',
        headers: { 
          'Authorization': `Bearer ${token}`
        }
      });
      
      if (res.ok) {
        fetchUsers();
        setAlertInfo({ type: 'success', message: 'Usuario eliminado correctamente.' });
      } else {
        let errorMessage = 'Error al eliminar el usuario';
        try {
            const text = await res.text();
            if (text) {
                const errorData = JSON.parse(text);
                errorMessage = errorData.message || errorMessage;
            }
        } catch (parseError) {
            console.error("Error parsing response", parseError);
        }
        setAlertInfo({ type: 'error', message: errorMessage });
      }
    } catch (error) {
      console.error(error);
      setAlertInfo({ type: 'error', message: 'Error de conexión al eliminar.' });
    }
  };

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const token = localStorage.getItem('token');
      const res = await fetch(`http://localhost:5219/api/auth/register`, {
        method: 'POST',
        headers: { 
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          workshopId: 1, // Default workshop
          roleName: newUser.roleName,
          firstName: newUser.firstName,
          lastName: newUser.lastName,
          email: newUser.email,
          phone: newUser.phone,
          password: newUser.password
        })
      });
      
      if (res.ok) {
        setIsCreating(false);
        setNewUser({ firstName: '', lastName: '', email: '', phone: '', password: '', roleName: 'Cliente' });
        fetchUsers();
        setAlertInfo({ type: 'success', message: 'Usuario creado exitosamente.' });
      } else {
        let errorMessage = 'Error al crear el usuario';
        try {
            const text = await res.text();
            if (text) {
                const errorData = JSON.parse(text);
                errorMessage = errorData.message || errorMessage;
            }
        } catch (parseError) {
            console.error("Error parsing response", parseError);
        }
        setAlertInfo({ type: 'error', message: errorMessage });
      }
    } catch (error: any) {
      console.error(error);
      setAlertInfo({ type: 'error', message: 'Error de conexión o de red: ' + error.message });
    }
  };

  return (
    <div className="space-y-6 relative">
      {/* ALERTA FLOTANTE (Toast) */}
      <div 
        className={`fixed top-6 left-1/2 -translate-x-1/2 z-[100] transition-all duration-500 ease-out flex items-center gap-3 px-5 py-3 rounded-2xl shadow-2xl backdrop-blur-md border ${
          alertInfo.type === 'error' 
            ? 'bg-red-500/10 border-red-500/20 text-red-700 translate-y-0 opacity-100' 
            : alertInfo.type === 'success'
            ? 'bg-green-500/10 border-green-500/20 text-green-700 translate-y-0 opacity-100'
            : '-translate-y-20 opacity-0 pointer-events-none'
        }`}
      >
        {alertInfo.type === 'error' && <AlertCircle className="w-5 h-5 text-red-500/80" />}
        {alertInfo.type === 'success' && <CheckCircle2 className="w-5 h-5 text-green-500/80" />}
        <span className="font-medium text-sm drop-shadow-sm">{alertInfo.message}</span>
        <button onClick={() => setAlertInfo({ type: null, message: '' })} className="ml-2 opacity-50 hover:opacity-100 transition-opacity">
          <X className="w-4 h-4" />
        </button>
      </div>

      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h2 className="text-2xl font-black tracking-tight text-gray-900">Gestión de Usuarios</h2>
          <p className="text-sm text-gray-500 font-medium">Administra roles, accesos y datos personales.</p>
        </div>
        
        <div className="flex flex-col sm:flex-row gap-3 w-full md:w-auto">
          <div className="relative w-full sm:w-80">
            <input
              type="text"
              placeholder="Buscar por ID, Email o Nombre..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              onKeyDown={(e) => e.key === 'Enter' && fetchUsers()}
              className="w-full pl-10 pr-4 py-2.5 bg-white border border-gray-200 rounded-xl focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 shadow-sm"
            />
            <Search className="absolute left-3 top-3 w-5 h-5 text-gray-400" />
            <button 
              onClick={fetchUsers}
              className="absolute right-2 top-1.5 px-3 py-1 bg-indigo-50 text-indigo-700 text-sm font-bold rounded-lg hover:bg-indigo-100"
            >
              Buscar
            </button>
          </div>
          <button 
            onClick={() => setIsCreating(true)}
            className="px-5 py-2.5 bg-indigo-600 text-white font-bold rounded-xl hover:bg-indigo-700 transition-colors shadow-sm whitespace-nowrap"
          >
            + Nuevo Usuario
          </button>
        </div>
      </div>

      <div className="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm text-gray-500">
            <thead className="bg-gray-50 text-gray-700 text-xs uppercase font-bold">
              <tr>
                <th className="px-6 py-4">Usuario</th>
                <th className="px-6 py-4">Email</th>
                <th className="px-6 py-4">Teléfono</th>
                <th className="px-6 py-4">Rol</th>
                <th className="px-6 py-4">Estado</th>
                <th className="px-6 py-4 text-right">Acciones</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {loading ? (
                <tr><td colSpan={6} className="p-8 text-center">Cargando...</td></tr>
              ) : users.length === 0 ? (
                <tr><td colSpan={6} className="p-8 text-center">No se encontraron usuarios.</td></tr>
              ) : (
                users.map(u => (
                  <tr key={u.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-6 py-4">
                      <div className="flex items-center gap-3">
                        <div className="w-8 h-8 rounded-full bg-indigo-100 flex items-center justify-center text-indigo-700 font-bold">
                          {u.firstName ? u.firstName[0] : u.email[0].toUpperCase()}
                        </div>
                        <div>
                          <p className="font-bold text-gray-900">{u.firstName} {u.lastName}</p>
                          <p className="text-xs text-gray-400">ID: {u.id}</p>
                        </div>
                      </div>
                    </td>
                    <td className="px-6 py-4 font-medium text-gray-600">{u.email}</td>
                    <td className="px-6 py-4 font-medium text-gray-600">{u.phone || '—'}</td>
                    <td className="px-6 py-4">
                      <span className={`px-2.5 py-1 rounded-full text-xs font-bold flex w-fit items-center gap-1
                        ${u.role === 'Admin' ? 'bg-purple-100 text-purple-700' : 
                          u.role === 'Mecanico' ? 'bg-blue-100 text-blue-700' : 'bg-gray-100 text-gray-700'}`}>
                        <Shield className="w-3 h-3" /> {u.role || 'Sin Rol'}
                      </span>
                    </td>
                    <td className="px-6 py-4">
                      {u.isActive ? (
                        <span className="flex items-center gap-1 text-emerald-600 font-bold text-xs"><CheckCircle className="w-4 h-4" /> Activo</span>
                      ) : (
                        <span className="flex items-center gap-1 text-red-500 font-bold text-xs"><XCircle className="w-4 h-4" /> Inactivo</span>
                      )}
                    </td>
                    <td className="px-6 py-4 text-right space-x-2">
                      <button 
                        onClick={() => setEditingUser(u)}
                        className="p-2 text-indigo-600 hover:bg-indigo-50 rounded-lg transition-colors inline-flex"
                        title="Editar usuario"
                      >
                        <Edit2 className="w-4 h-4" />
                      </button>
                      <button 
                        onClick={() => setUserToDelete(u.id)}
                        className="p-2 text-red-500 hover:bg-red-50 rounded-lg transition-colors inline-flex"
                        title="Eliminar usuario"
                      >
                        <UserX className="w-4 h-4" />
                      </button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>

      {/* Delete Confirmation Modal */}
      {userToDelete !== null && (
        <div className="fixed inset-0 bg-gray-900/50 backdrop-blur-sm flex items-center justify-center p-4 z-[200]">
          <div className="bg-white rounded-3xl shadow-2xl w-full max-w-sm overflow-hidden p-6 text-center space-y-4">
            <div className="w-16 h-16 bg-red-100 text-red-500 rounded-full flex items-center justify-center mx-auto mb-4">
              <UserX className="w-8 h-8" />
            </div>
            <h3 className="text-xl font-bold text-gray-800">Eliminar Usuario</h3>
            <p className="text-gray-500 font-medium">
              ¿Estás seguro de que deseas eliminar este usuario de forma permanente? Esta acción no se puede deshacer.
            </p>
            <div className="flex gap-3 pt-4">
              <button 
                onClick={() => setUserToDelete(null)}
                className="flex-1 py-3 px-4 bg-gray-100 hover:bg-gray-200 text-gray-800 font-bold rounded-xl transition-colors"
              >
                Cancelar
              </button>
              <button 
                onClick={() => executeDelete(userToDelete)}
                className="flex-1 py-3 px-4 bg-red-500 hover:bg-red-600 text-white font-bold rounded-xl transition-colors shadow-lg shadow-red-500/30"
              >
                Eliminar
              </button>
            </div>
          </div>
        </div>
      )}

      {isCreating && (
        <div className="fixed inset-0 bg-gray-900/50 backdrop-blur-sm flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-3xl shadow-2xl w-full max-w-md overflow-hidden">
            <div className="p-6 border-b border-gray-100 bg-gray-50 flex justify-between items-center">
              <h3 className="text-xl font-bold text-gray-800">Nuevo Usuario</h3>
              <button onClick={() => setIsCreating(false)} className="text-gray-400 hover:text-gray-700">
                <XCircle className="w-6 h-6" />
              </button>
            </div>
            
            <form onSubmit={handleCreate} className="p-6 space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-bold text-gray-700 mb-1">Nombres</label>
                  <input type="text" required value={newUser.firstName} onChange={e => setNewUser({...newUser, firstName: e.target.value})} className="w-full px-4 py-2 border border-gray-200 rounded-xl focus:ring-2 focus:ring-indigo-500" />
                </div>
                <div>
                  <label className="block text-sm font-bold text-gray-700 mb-1">Apellidos</label>
                  <input type="text" required value={newUser.lastName} onChange={e => setNewUser({...newUser, lastName: e.target.value})} className="w-full px-4 py-2 border border-gray-200 rounded-xl focus:ring-2 focus:ring-indigo-500" />
                </div>
              </div>

              <div>
                <label className="block text-sm font-bold text-gray-700 mb-1">Correo Electrónico</label>
                <input type="email" required value={newUser.email} onChange={e => setNewUser({...newUser, email: e.target.value})} className="w-full px-4 py-2 border border-gray-200 rounded-xl focus:ring-2 focus:ring-indigo-500" />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-bold text-gray-700 mb-1">Teléfono</label>
                  <input type="tel" value={newUser.phone} onChange={e => setNewUser({...newUser, phone: e.target.value})} className="w-full px-4 py-2 border border-gray-200 rounded-xl focus:ring-2 focus:ring-indigo-500" />
                </div>
                <div>
                  <label className="block text-sm font-bold text-gray-700 mb-1">Contraseña</label>
                  <input type="password" required minLength={6} value={newUser.password} onChange={e => setNewUser({...newUser, password: e.target.value})} className="w-full px-4 py-2 border border-gray-200 rounded-xl focus:ring-2 focus:ring-indigo-500" />
                </div>
              </div>

              <div>
                <label className="block text-sm font-bold text-gray-700 mb-1">Rol del Sistema</label>
                <select value={newUser.roleName} onChange={e => setNewUser({...newUser, roleName: e.target.value})} className="w-full px-4 py-2 border border-gray-200 rounded-xl focus:ring-2 focus:ring-indigo-500 bg-white">
                  <option value="Cliente">Cliente</option>
                  <option value="Mecanico">Mecánico</option>
                  <option value="Recepcionista">Recepcionista</option>
                  <option value="Admin">Administrador</option>
                </select>
              </div>

              <div className="pt-4 flex gap-3">
                <button type="button" onClick={() => setIsCreating(false)} className="flex-1 px-4 py-2 bg-gray-100 text-gray-700 font-bold rounded-xl hover:bg-gray-200 transition-colors">
                  Cancelar
                </button>
                <button type="submit" className="flex-1 px-4 py-2 bg-indigo-600 text-white font-bold rounded-xl hover:bg-indigo-700 transition-colors">
                  Crear Usuario
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {editingUser && (
        <div className="fixed inset-0 bg-gray-900/50 backdrop-blur-sm flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-3xl shadow-2xl w-full max-w-md overflow-hidden">
            <div className="p-6 border-b border-gray-100 bg-gray-50 flex justify-between items-center">
              <h3 className="text-xl font-bold text-gray-800">Editar Usuario</h3>
              <button onClick={() => setEditingUser(null)} className="text-gray-400 hover:text-gray-700">
                <XCircle className="w-6 h-6" />
              </button>
            </div>
            
            <form onSubmit={handleUpdate} className="p-6 space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-bold text-gray-700 mb-1">Nombres</label>
                  <input type="text" required value={editingUser.firstName} onChange={e => setEditingUser({...editingUser, firstName: e.target.value})} className="w-full px-4 py-2 border border-gray-200 rounded-xl focus:ring-2 focus:ring-indigo-500" />
                </div>
                <div>
                  <label className="block text-sm font-bold text-gray-700 mb-1">Apellidos</label>
                  <input type="text" required value={editingUser.lastName} onChange={e => setEditingUser({...editingUser, lastName: e.target.value})} className="w-full px-4 py-2 border border-gray-200 rounded-xl focus:ring-2 focus:ring-indigo-500" />
                </div>
              </div>

              <div>
                <label className="block text-sm font-bold text-gray-700 mb-1">Teléfono</label>
                <input type="tel" value={editingUser.phone || ''} onChange={e => setEditingUser({...editingUser, phone: e.target.value})} className="w-full px-4 py-2 border border-gray-200 rounded-xl focus:ring-2 focus:ring-indigo-500" />
              </div>

              <div>
                <label className="block text-sm font-bold text-gray-700 mb-1">Rol del Sistema</label>
                <select value={editingUser.role} onChange={e => setEditingUser({...editingUser, role: e.target.value})} className="w-full px-4 py-2 border border-gray-200 rounded-xl focus:ring-2 focus:ring-indigo-500 bg-white">
                  <option value="Cliente">Cliente</option>
                  <option value="Mecanico">Mecánico</option>
                  <option value="Recepcionista">Recepcionista</option>
                  <option value="Admin">Administrador</option>
                </select>
              </div>

              <div className="flex items-center gap-3 pt-2">
                <label className="relative inline-flex items-center cursor-pointer">
                  <input type="checkbox" className="sr-only peer" checked={editingUser.isActive} onChange={e => setEditingUser({...editingUser, isActive: e.target.checked})} />
                  <div className="w-11 h-6 bg-gray-200 peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-indigo-600"></div>
                </label>
                <span className="text-sm font-bold text-gray-700">Cuenta Activa</span>
              </div>

              <div className="pt-4 flex gap-3">
                <button type="button" onClick={() => setEditingUser(null)} className="flex-1 px-4 py-2 bg-gray-100 text-gray-700 font-bold rounded-xl hover:bg-gray-200 transition-colors">
                  Cancelar
                </button>
                <button type="submit" className="flex-1 px-4 py-2 bg-indigo-600 text-white font-bold rounded-xl hover:bg-indigo-700 transition-colors">
                  Guardar Cambios
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
