export interface IcourseProfessor {
    id: number;
    fullName: string;
    email: string;
  }
  
  export type CourseEtat = 'PUBLISHED' | 'DRAFT';
  
  export interface Icours {
    id: number;
    titre: string;
    etat: CourseEtat;
    datePublication: string | null;
    categoryId: number;
    nomFichierPdf: string | null;
    professors: IcourseProfessor[];
  }
  