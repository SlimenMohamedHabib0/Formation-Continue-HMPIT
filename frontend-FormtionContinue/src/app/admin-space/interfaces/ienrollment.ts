export interface Ienrollment {
    id: number;
  
    courseId: number;
    courseTitre: string;
    categoryId: number;
  
    userId: number;
    userFullName: string;
    userEmail: string;
  
    statut: string;
    dateDemande: string;
    dateDecision: string | null;
  
    decisionProfessorId: number | null;
    decisionProfessorFullName: string | null;
    decisionProfessorEmail: string | null;
  }
  