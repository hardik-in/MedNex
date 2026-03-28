export interface TimeSlot {
  id: number;
  doctorId: number;
  doctorName?: string;
  date: string;
  startTime: string;
  endTime: string;
  durationMinutes: number;
  status: number; 
  createdAt?: string;
}
