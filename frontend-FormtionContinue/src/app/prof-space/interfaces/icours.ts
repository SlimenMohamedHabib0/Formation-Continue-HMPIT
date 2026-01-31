export interface ICours {
    id: number;
    titre: string;
    description: string;
    motsCles: string;
    etat: string;
    datePublication: string | null;
    nomFichierPdf: string | null;
    categoryId: number;
  }
  