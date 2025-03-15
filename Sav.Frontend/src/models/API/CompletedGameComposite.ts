import { UserGameDto } from "./UserGameDto";
import { GameDto } from "./GameDto";
import { AchievementDto } from "./AchievementDto";

export interface CompletedGameComposite {
    userGame: UserGameDto;
    game: GameDto;
    achievements: AchievementDto[];
    completedAt: string;
    hoursPlayed: string;
    previewAchievements: AchievementDto[];
    previewCount: number;
    previewCountString: string;
    logoUrl: string;
}