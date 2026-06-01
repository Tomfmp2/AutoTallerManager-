import { Outlet, NavLink, useNavigate } from 'react-router-dom';
import { LayoutDashboard, Users, Car, CalendarClock, Settings, LogOut, Package } from 'lucide-react';
import LanguageSwitcher from '../components/LanguageSwitcher';
import NotificationDropdown from '../components/NotificationDropdown';

const navItems = [
  { to: '/admin/dashboard',       icon: <LayoutDashboard className="w-5 h-5" />, label: 'Dashboard' },
  { to: '/admin/usuarios',        icon: <Users className="w-5 h-5" />,           label: 'Gestión Usuarios' },
  { to: '/admin/vehiculos',       icon: <Car className="w-5 h-5" />,             label: 'Control Vehículos' },
  { to: '/admin/Revisiones',           icon: <CalendarClock className="w-5 h-5" />,   label: 'Revisiones y Ã“rdenes' },
  { to: '/admin/inventario',      icon: <Package className="w-5 h-5" />,         label: 'Inventario' },
  { to: '/admin/configuracion',   icon: <Settings className="w-5 h-5" />,        label: 'Configuración' },
];

export default function AdminLayout() {
  const navigate = useNavigate();
  const email = localStorage.getItem('userEmail') || 'Administrador';
  const displayName = email.includes('@') ? email.split('@')[0] : email;

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('userEmail');
    localStorage.removeItem('userRole');
    navigate('/login');
  };

  return (
    <div className="min-h-screen bg-[#F0F2F5] flex font-sans text-gray-800">
      
      {/* Sidebar */}
      <aside className="w-64 bg-white border-r border-gray-200 flex flex-col fixed h-full z-10 shadow-sm">
        <div className="h-20 flex items-center px-8 border-b border-gray-100">
          <h1 className="text-2xl font-bold text-gray-800 tracking-tight flex items-center gap-2">
            <div className="p-2 bg-gradient-to-tr from-blue-600 to-indigo-600 rounded-lg text-white">
              <Car className="w-5 h-5" />
            </div>
            AutoTaller<span className="text-blue-600">Pro</span>
          </h1>
        </div>

        <nav className="flex-1 px-4 py-4 space-y-2 overflow-y-auto">
          <div className="mb-4 px-2 text-xs font-bold text-gray-400 uppercase tracking-wider">Principal</div>
          {navItems.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) =>
                `flex items-center gap-3 px-3 py-2.5 rounded-xl transition-all duration-200 group ${
                  isActive
                    ? 'bg-indigo-50 text-indigo-700 font-semibold'
                    : 'text-gray-500 hover:bg-gray-50 hover:text-gray-900'
                }`
              }
            >
              <div className="transition-transform group-hover:scale-110">
                {item.icon}
              </div>
              <span>{item.label}</span>
            </NavLink>
          ))}
        </nav>

        <div className="p-4 border-t border-gray-100">
          <button
            onClick={handleLogout}
            className="flex items-center gap-3 px-3 py-2.5 w-full text-red-500 hover:bg-red-50 rounded-xl transition-all font-medium"
          >
            <LogOut className="w-5 h-5" />
            <span>Cerrar Sesión</span>
          </button>
        </div>
      </aside>

      {/* Main Content (Offset by Sidebar Width) */}
      <div className="flex-1 ml-64 flex flex-col min-h-screen">
        {/* Top Header */}
        <header className="h-20 bg-white/80 backdrop-blur-md border-b border-gray-200 sticky top-0 z-20 px-8 flex items-center justify-between">
          <h2 className="text-2xl font-bold text-gray-800 tracking-tight">Administración Global</h2>
          
          <div className="flex items-center gap-5">
            <LanguageSwitcher />
            
            <div className="h-8 w-px bg-gray-200"></div>
            
            <NotificationDropdown />

            <div className="flex items-center gap-3 pl-2">
              <div className="text-right hidden md:block">
                <p className="text-sm font-bold text-gray-800">{displayName}</p>
                <p className="text-xs text-indigo-600 font-medium">Administrador</p>
              </div>
              <div className="w-10 h-10 rounded-xl bg-gradient-to-tr from-indigo-600 to-purple-600 text-white flex items-center justify-center font-bold shadow-md ring-2 ring-white cursor-pointer hover:scale-105 transition-transform">
                {displayName.charAt(0).toUpperCase()}
              </div>
            </div>
          </div>
        </header>

        {/* Page Content */}
        <main className="flex-1 p-8 overflow-y-auto">
          <div className="max-w-7xl mx-auto">
            <Outlet />
          </div>
        </main>
      </div>
    </div>
  );
}


