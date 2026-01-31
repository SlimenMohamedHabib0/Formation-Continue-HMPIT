import { Ichoix } from './ichoix';

export interface Iquestion {
  id: number;
  enonce: string;
  points: number;
  choix: Ichoix[];
}

export interface IquestionCreateDto {
  enonce: string;
  points: number;
  choix: Ichoix[];
}

export interface IquestionUpdateDto {
  enonce: string;
  points: number;
  choix: Ichoix[];
}

export interface IQcmValidityDto {
  totalPoints: number;
  isValid: boolean;
  warnings: string[];
}
