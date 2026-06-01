import { Navigate, Outlet } from 'react-router-dom';

interface ProtectedRouteProps {
  allowedRoles?: string[];
}

export default function ProtectedRoute({ allowedRoles }: ProtectedRouteProps) {
  const token = localStorage.getItem('token');
  const userRole = localStorage.getItem('userRole');

  if (!token) {
    return <Navigate to="/login" replace />;
  }

  if (allowedRoles && allowedRoles.length > 0) {
    if (!userRole || !allowedRoles.includes(userRole)) {
      // Redirect to a safe place depending on their actual role, or to client dashboard as default
      if (userRole === 'Administrador' || userRole === 'Admin') return <Navigate to="/admin" replace />;
      return <Navigate to="/cliente/dashboard" replace />;
    }
  }

  return <Outlet />;
}
