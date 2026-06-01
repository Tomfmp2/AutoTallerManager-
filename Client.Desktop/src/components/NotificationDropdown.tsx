import React, { useState, useEffect, useRef } from 'react';
import { Bell, Check, Trash2, CalendarX, Clock, Car } from 'lucide-react';

interface NotificationDto {
  id: number;
  title: string;
  message: string;
  createdAt: string;
}

export default function NotificationDropdown() {
  const [isOpen, setIsOpen] = useState(false);
  const [notifications, setNotifications] = useState<NotificationDto[]>([]);
  const dropdownRef = useRef<HTMLDivElement>(null);

  const fetchNotifications = async () => {
    const token = localStorage.getItem('token');
    if (!token) return;
    try {
      const res = await fetch('http://localhost:5219/api/dashboard/client/notifications', {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      if (res.ok) {
        setNotifications(await res.json());
      }
    } catch (e) {
      console.error(e);
    }
  };

  useEffect(() => {
    fetchNotifications();
    // Poll every 30 seconds
    const interval = setInterval(fetchNotifications, 30000);
    return () => clearInterval(interval);
  }, []);

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const markAsRead = async (id: number) => {
    const token = localStorage.getItem('token');
    if (!token) return;
    try {
      const res = await fetch(`http://localhost:5219/api/dashboard/client/notifications/${id}/read`, {
        method: 'POST',
        headers: { 'Authorization': `Bearer ${token}` }
      });
      if (res.ok) {
        setNotifications(prev => prev.filter(n => n.id !== id));
      }
    } catch (e) {
      console.error(e);
    }
  };

  const getIcon = (title: string) => {
    if (title.toLowerCase().includes('vencida')) return <CalendarX className="w-5 h-5 text-red-500" />;
    if (title.toLowerCase().includes('tiempo')) return <Clock className="w-5 h-5 text-yellow-500" />;
    if (title.toLowerCase().includes('listo')) return <Car className="w-5 h-5 text-green-500" />;
    return <Bell className="w-5 h-5 text-blue-500" />;
  };

  return (
    <div className="relative" ref={dropdownRef}>
      <button 
        onClick={() => setIsOpen(!isOpen)}
        className="relative p-2.5 bg-white rounded-full shadow-sm border border-gray-100 text-gray-400 hover:text-blue-600 hover:shadow-md transition-all cursor-pointer"
      >
        <Bell className="w-5 h-5" />
        {notifications.length > 0 && (
          <span className="absolute top-1 right-1 w-2.5 h-2.5 bg-red-500 rounded-full border-2 border-white"></span>
        )}
      </button>

      {isOpen && (
        <div className="absolute right-0 mt-3 w-80 bg-white rounded-2xl shadow-xl border border-gray-100 overflow-hidden z-50 transform opacity-100 scale-100 transition-all origin-top-right">
          <div className="p-4 bg-gray-50/50 border-b border-gray-100 flex justify-between items-center">
            <h3 className="font-bold text-gray-800">Notificaciones</h3>
            <span className="text-xs font-medium px-2 py-1 bg-blue-100 text-blue-700 rounded-lg">
              {notifications.length} nuevas
            </span>
          </div>

          <div className="max-h-[350px] overflow-y-auto">
            {notifications.length === 0 ? (
              <div className="p-8 text-center text-gray-400">
                <Bell className="w-8 h-8 mx-auto mb-2 opacity-20" />
                <p className="text-sm">No tienes notificaciones nuevas</p>
              </div>
            ) : (
              <div className="divide-y divide-gray-50">
                {notifications.map(n => (
                  <div key={n.id} className="p-4 hover:bg-gray-50 transition-colors flex gap-3 group">
                    <div className="mt-0.5">{getIcon(n.title)}</div>
                    <div className="flex-1">
                      <h4 className="text-sm font-bold text-gray-800">{n.title}</h4>
                      <p className="text-xs text-gray-600 mt-1 leading-relaxed">{n.message}</p>
                      <p className="text-[10px] text-gray-400 mt-2 font-medium">
                        {new Date(n.createdAt).toLocaleString()}
                      </p>
                    </div>
                    <button 
                      onClick={() => markAsRead(n.id)}
                      className="opacity-0 group-hover:opacity-100 p-1.5 h-fit text-gray-400 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition-all"
                      title="Marcar como leída"
                    >
                      <Check className="w-4 h-4" />
                    </button>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      )}
    </div>
  );
}
