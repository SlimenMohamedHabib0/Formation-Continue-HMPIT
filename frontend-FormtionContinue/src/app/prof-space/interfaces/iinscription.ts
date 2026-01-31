export interface IInscription {
  id: number;
  courseId: number;
  courseTitre: string;
  userId: number;
  userFullName: string;
  userEmail: string;
  dateInscription: string;
  statut: 'PENDING' | 'ACCEPTEE' | 'REFUSEE' | string;
}