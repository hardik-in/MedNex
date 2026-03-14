export interface Doctor {
  id: number;
  name: string;
  specialization: string;
}
export interface UpdateDoctorDto {
  phoneNumber?: string;
  specialization?: string;
  yearsOfExperience?: number;
  qualifications?: string;
  bio?: string;
  consultationFee?: number;
  address?: string;
}