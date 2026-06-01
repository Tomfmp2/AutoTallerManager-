import { Outlet, NavLink, useNavigate } from 'react-router-dom';
import { 
  LogOut, 
  LayoutDashboard, 
  CheckSquare, 
  Users, 
  Car, 
  Calendar
} from 'lucide-react';
import LanguageSwitcher from '../components/LanguageSwitcher';
import NotificationDropdown from '../components/NotificationDropdown';

const navItems = [
  { to: '/recepcion/dashboard', icon: <LayoutDashboard className="w-5 h-5" />, label: 'Dashboard' },
  { to: '/recepcion/aprobaciones', icon: <CheckSquare className="w-5 h-5" />, label: 'Aprobaciones' },
  { to: '/recepcion/clientes', icon: <Users className="w-5 h-5" />, label: 'Clientes' },
  { to: '/recepcion/vehiculos', icon: <Car className="w-5 h-5" />, label: 'Vehículos' },
  { to: '/recepcion/Revisiones', icon: <Calendar className="w-5 h-5" />, label: 'Revisiones' },
];

export default function RecepcionLayout() {
  const navigate = useNavigate();
  const email = localStorage.getItem('userEmail') || 'Recepcionista';
  const displayName = email.includes('@') ? email.split('@')[0] : email;

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('userRole');
    localStorage.removeItem('userName');
    localStorage.removeItem('userEmail');
    navigate('/login');
  };

  return (
    <div className="min-h-screen bg-[#F8F9FA] flex font-sans text-gray-800">
      {/* Sidebar */}
      <aside className="w-64 bg-white border-r border-gray-100 flex-col hidden md:flex shrink-0 shadow-sm z-10">
        <div className="h-20 flex items-center px-8 border-b border-gray-50">
          <h1 className="text-2xl font-bold text-gray-800 tracking-tight flex items-center gap-2">
            <div className="p-2 bg-gradient-to-tr from-blue-600 to-indigo-600 rounded-lg text-white">
              <Car className="w-5 h-5" />
            </div>
            AutoTaller<span className="text-blue-600">Pro</span>
          </h1>
        </div>

        <nav className="flex-1 py-8 px-4 space-y-1">
          {navItems.map(({ to, icon, label }) => (
            <NavLink
              key={to}
              to={to}
              className={({ isActive }) =>
                `flex items-center gap-3 px-4 py-3 rounded-xl font-medium transition-all duration-200 ${
                  isActive
                    ? 'bg-blue-50 text-blue-700 font-semibold shadow-sm'
                    : 'text-gray-500 hover:bg-gray-50 hover:text-gray-900'
                }`
              }
            >
              {icon}
              {label}
            </NavLink>
          ))}
        </nav>

        <div className="p-4 border-t border-gray-50">
          <button
            onClick={handleLogout}
            className="flex items-center justify-center gap-2 w-full px-4 py-3 text-red-500 hover:bg-red-50 rounded-xl font-medium transition-colors"
          >
            <LogOut className="w-5 h-5" />
            Cerrar Sesión
          </button>
        </div>
      </aside>

      {/* Main content */}
      <main className="flex-1 flex flex-col h-screen overflow-hidden">
        {/* Top Header */}
        <header className="h-20 bg-white/80 backdrop-blur-md border-b border-gray-100 flex items-center justify-between px-8 shrink-0 shadow-sm z-10">
          <div>
            <h2 className="text-xl font-semibold text-gray-800">Portal Recepción</h2>
            <p className="text-sm text-gray-500">Bienvenido/a de nuevo, {displayName}</p>
          </div>

          <div className="flex items-center gap-4">
            <LanguageSwitcher />
            
            <NotificationDropdown />

            <div
              className="w-10 h-10 rounded-full bg-gradient-to-tr from-emerald-400 to-teal-500 flex items-center justify-center text-white font-bold shadow-md"
            >
              {displayName.charAt(0).toUpperCase()}
            </div>
          </div>
        </header>

        {/* Page content */}
        <div className="flex-1 overflow-auto bg-[#F8F9FA]">
          <Outlet />
        </div>
      </main>
    </div>
  );
}


