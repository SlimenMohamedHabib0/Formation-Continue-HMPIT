
export interface IInscription {
  id: number;

  courseId: number;
  courseTitre: string;

  userId: number;
  userFullName: string;
  userEmail: string;

  serviceId: number;
  serviceLibelle: string;
  statutId: number;
  statutLibelle: string;

  statut: 'PENDING' | 'ACCEPTEE' | 'REFUSEE' | string;

  dateDemande: string;
  dateDecision?: string | null;
}
