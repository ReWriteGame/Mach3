using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class BoardManager : MonoBehaviour
{
	public static BoardManager instance;

	[SerializeField] private List<Sprite> characters = new List<Sprite>();
	[SerializeField] private GameObject tile;
	[SerializeField] private Vector2Int sizeBoard;

	private GameObject[,] tiles;

	public UnityEvent OnMatеcedCells;

	public bool IsShifting { get; set; }

    private void Start()
	{
		instance = GetComponent<BoardManager>();
		Vector2 cellSize = tile.GetComponent<BoxCollider2D>().size;
		CreateBoard(cellSize);
	}

	private void CreateBoard(Vector2 cellSize)
	{
		tiles = new GameObject[sizeBoard.x, sizeBoard.y];

		float startX = transform.position.x;
		float startY = transform.position.y;

		Sprite[] previousLeft = new Sprite[sizeBoard.y]; // Add this line
		Sprite previousBelow = null; // Add this line

		for (int x = 0; x < sizeBoard.x; x++)
		{
			for (int y = 0; y < sizeBoard.y; y++)
			{
				Vector3 spawnTilePosition = new Vector3(startX + (cellSize.x * x), startY + (cellSize.y * y), 0);
				GameObject newTile = Instantiate(tile, spawnTilePosition, tile.transform.rotation);
				newTile.transform.parent = transform;

				tiles[x, y] = newTile;

				List<Sprite> possibleCharacters = new List<Sprite>();
				possibleCharacters.AddRange(characters);

				possibleCharacters.Remove(previousLeft[y]);
				possibleCharacters.Remove(previousBelow);

				Sprite newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)];
				newTile.GetComponent<SpriteRenderer>().sprite = newSprite;
				previousLeft[y] = newSprite;
				previousBelow = newSprite;
			}
		}
	}

	public IEnumerator FindNullTiles()
	{
		for (int x = 0; x < sizeBoard.x; x++)
		{
			for (int y = 0; y < sizeBoard.y; y++)
			{
				if (tiles[x, y].GetComponent<SpriteRenderer>().sprite == null)
				{
					yield return StartCoroutine(ShiftTilesDown(x, y));
					break;
				}
			}
		}

		for (int x = 0; x < sizeBoard.x; x++)
			for (int y = 0; y < sizeBoard.y; y++)
				tiles[x, y].GetComponent<Tile>().ClearAllMatches();
	}

	private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay = .03f)
	{
		IsShifting = true;
		List<SpriteRenderer> renders = new List<SpriteRenderer>();
		int nullCount = 0;

		for (int y = yStart; y < sizeBoard.y; y++)
		{
			SpriteRenderer render = tiles[x, y].GetComponent<SpriteRenderer>();
			if (render.sprite == null)
			{
				nullCount++;
			}
			renders.Add(render);
		}

		for (int i = 0; i < nullCount; i++)
		{
			OnMatеcedCells?.Invoke();
			yield return new WaitForSeconds(shiftDelay);
			for (int k = 0; k < renders.Count - 1; k++)
			{
				renders[k].sprite = renders[k + 1].sprite;
				renders[k + 1].sprite = GetNewSprite(x, sizeBoard.y - 1);
			}
		}
		IsShifting = false;
	}

	private Sprite GetNewSprite(int x, int y)
	{
		List<Sprite> possibleCharacters = new List<Sprite>();
		possibleCharacters.AddRange(characters);

		if (x > 0)
		{
			possibleCharacters.Remove(tiles[x - 1, y].GetComponent<SpriteRenderer>().sprite);
		}
		if (x < sizeBoard.x - 1)
		{
			possibleCharacters.Remove(tiles[x + 1, y].GetComponent<SpriteRenderer>().sprite);
		}
		if (y > 0)
		{
			possibleCharacters.Remove(tiles[x, y - 1].GetComponent<SpriteRenderer>().sprite);
		}

		return possibleCharacters[Random.Range(0, possibleCharacters.Count)];
	}
}
